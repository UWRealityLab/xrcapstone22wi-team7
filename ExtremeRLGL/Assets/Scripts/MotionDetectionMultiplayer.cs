using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
using TMPro;
using UnityEngine.SceneManagement;


public class MotionDetectionMultiplayer : MonoBehaviour
{

    private PhotonView photonView;

    // Headset and controller game objects
    public Transform MainCamera;
    public Transform LeftHand;
    public Transform RightHand;

    // Initial position coordinates
    private Vector3 initCameraPos;
    private Vector3 initLeftPos;
    private Vector3 initRightPos;

    // Initial rotation coordinates
    private Vector3 initCameraRot;
    private Vector3 initLeftRot;
    private Vector3 initRightRot;

    // Current position coordinates
    private Vector3 currCameraPos;
    private Vector3 currLeftPos;
    private Vector3 currRightPos;

    // Current rotation coordinates
    private Vector3 currCameraRot;
    private Vector3 currLeftRot;
    private Vector3 currRightRot;

    // Position thresholds
    public float cameraPosThreshold;
    public float handPosThreshold;

    // Rotation thresholds
    public float cameraRotThreshold;
    public float handRotThreshold;

    private bool resetPosition;

    private PlayerInteraction playerInteraction;
    private ClimbingMovement climbingMovement;
    private PlayerPowerup playerPowerup;
    private Collider startLine;
    public TextMeshProUGUI movingState;
    private bool triggered;
    private XROrigin rig;

    // Animator
    public Animator animator;

    // Audio
    public AudioSource audioSource;
    public AudioClip dingSound;
    public AudioClip bzzSound;
    public bool alreadyPlayingAudio;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        rig = FindObjectOfType<XROrigin>();
        MainCamera = rig.transform.Find("Camera Offset/Main Camera");
        LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
        climbingMovement = rig.GetComponent<ClimbingMovement>();
        playerInteraction = gameObject.GetComponent<PlayerInteraction>();
        playerPowerup = gameObject.GetComponent<PlayerPowerup>();
        if (photonView.IsMine)
            movingState.text = "";
        triggered = false;
        resetPosition = false;

        audioSource = GameObject.Find("Main Camera").GetComponent<AudioSource>();
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    } 

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        Debug.Log(scene.name);
        if (scene.name == "MultiplayerGameScene")
        {
            Debug.Log("Initializing on scene loaded");
            startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
            if (startLine == null)
            {
                Debug.LogError("Start line is empty!");
            }
            if (photonView.IsMine && Time.timeSinceLevelLoad > 1f)
            {
                ResetInitialPositions();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        audioSource = GameObject.Find("UI Camera").GetComponent<AudioSource>();

        if (startLine == null)
        {
            startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
            Debug.Log("Tried to find start line");
        }

        if (photonView.IsMine && startLine != null && GameManager.gameStage == GameStage.Playing && LightManager.RedlightAllOn())
        {
            if (!resetPosition)
            {
                ResetInitialPositions();
                resetPosition = true;
            }

            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            XROrigin rig = FindObjectOfType<XROrigin>();
            // NOTE: this part throws errors when player falls
            if (rig != null)
            {
                MainCamera = rig.transform.Find("Camera Offset/Main Camera");
                LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
                RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
                climbingMovement = rig.GetComponent<ClimbingMovement>();
            }

            // Get current position coordinates
            currCameraPos = MainCamera.position;
            currLeftPos = LeftHand.position;
            currRightPos = RightHand.position;

            // Get current rotation coordinates
            currCameraRot = MainCamera.rotation.eulerAngles;
            currLeftRot = LeftHand.rotation.eulerAngles;
            currRightRot = RightHand.rotation.eulerAngles;

            // Get distance between initial and current position coordinates
            float cameraPosDist = Vector3.Distance(initCameraPos, currCameraPos);
            float leftPosDist = Vector3.Distance(initLeftPos, currLeftPos);
            float rightPosDist = Vector3.Distance(initRightPos, currRightPos);

            // Get distance between initial and current rotation coordinates
            float cameraRotDist = Vector3.Distance(initCameraRot, currCameraRot);
            float leftRotDist = Vector3.Distance(initLeftRot, currLeftRot);
            float rightRotDist = Vector3.Distance(initRightRot, currRightRot);

            // Executes if calculated distances are greater than their respective thresholds
            if (cameraPosDist > cameraPosThreshold || leftPosDist > handPosThreshold || rightPosDist > handPosThreshold ||
                cameraRotDist > cameraRotThreshold || leftRotDist > handRotThreshold || rightRotDist > handRotThreshold)
            {
                if (!triggered && !playerPowerup.isActivate(PowerUpType.UNSTOPPABLE))
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(bzzSound);
                    alreadyPlayingAudio = true;
                    OnMoved();
                }
            }

            // Executes if calculated distances are less than their respective thresholds
            else
            {
                //Debug.Log("Freeze");
                // GetComponent<Renderer>().material.color = Color.red;
                if (!alreadyPlayingAudio)
                {
                    audioSource.Stop();
                    audioSource.PlayOneShot(dingSound);
                    alreadyPlayingAudio = true;
                }
            }
        }
        else if (!LightManager.RedlightAllOn())
        {
            resetPosition = false;
            alreadyPlayingAudio = false;
        }
    }


    public void ResetInitialPositions()
    {
        Debug.Log("Reset Archor Position");
        rig = FindObjectOfType<XROrigin>();
        MainCamera = rig.transform.Find("Camera Offset/Main Camera");
        LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
        
        // Get initial position coordinates
        initCameraPos = MainCamera.transform.position;
        initLeftPos = LeftHand.transform.position;
        initRightPos = RightHand.transform.position;

        // Get initial rotation coordinates
        initCameraRot = MainCamera.transform.rotation.eulerAngles;
        initLeftRot = LeftHand.transform.rotation.eulerAngles;
        initRightRot = RightHand.transform.rotation.eulerAngles;

        if (photonView.IsMine)
            movingState.text = " ";
    }

    private void OnMoved()
    {
        if (photonView.IsMine)
            movingState.text = "You moved!";
        playerInteraction.stopped = true;
        StartCoroutine(MovePlayer());
        triggered = true;
        if (rig == null)
            rig = FindObjectOfType<XROrigin>();
        rig.GetComponent<Flash>().StartFlash(Color.red, 1f);
        Debug.Log("You moved!");
    }

    IEnumerator MovePlayer()
    {
        yield return new WaitForSeconds(3.0f);
        if (photonView.IsMine)
            movingState.text = "";
        climbingMovement.Reset();
        playerInteraction.stopped = false;
        Teleport t = gameObject.GetComponent<Teleport>();
        if (t != null) // if it's a player
        {
            t.ToStartLine();
        }
        else // if it's a bot
        {
            gameObject.transform.position = startLine.GetComponent<StartLine>().GetNextPos();
        }
        ResetInitialPositions();
        triggered = false;
        Debug.Log("Has been punished.");
    }
}