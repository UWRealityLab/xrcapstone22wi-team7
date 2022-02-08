using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

public class RunningMovementMultiplayer : MonoBehaviour
{

    private PhotonView photonView;
    private NetworkPlayer networkPlayer;

    // Headset, controller, and container game objects
    public Transform LeftHand;
    public Transform RightHand;
    public Transform MainCamera;
    public GameObject LeftRunningContainer;
    public GameObject RightRunningContainer;

    // Initial position coordinates
    private Vector3 initLeftPos;
    private Vector3 initRightPos;
    private Vector3 initPlayerPos;

    // Current position coordinates
    private Vector3 currLeftPos;
    private Vector3 currRightPos;
    private Vector3 currPlayerPos;

    // Speed variable to determine how far the player moves forward
    public float speed = 80;

    private XROrigin rig;
    private bool leftOrRight = false;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        rig = FindObjectOfType<XROrigin>();
        MainCamera = rig.transform.Find("Camera Offset/Main Camera");
        LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        RightHand = rig.transform.Find("Camera Offset/RightHand Controller");

        networkPlayer = GetComponent<NetworkPlayer>();

        if (photonView.IsMine) 
        { 
            // Get initial position coordinates
            initPlayerPos = rig.transform.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;
        }
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
            LeftRunningContainer = GameObject.Find("LeftRunning");
            RightRunningContainer = GameObject.Find("RightRunning");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !networkPlayer.stopped && LeftRunningContainer != null && GameManager.gameStage == GameStage.Playing)
        {
            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            rig = FindObjectOfType<XROrigin>();
            if (rig != null)
            {
                MainCamera = rig.transform.Find("Camera Offset/Main Camera");
                LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
                RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
            }

            // Rotate the container depending on the rotation of the left controller
            LeftRunningContainer.transform.eulerAngles = new Vector3(0, LeftHand.transform.eulerAngles.y, 0);
            RightRunningContainer.transform.eulerAngles = new Vector3(0, RightHand.transform.eulerAngles.y, 0);

            // Get current position coordinates
            currLeftPos = LeftHand.position;
            currRightPos = RightHand.position;
            currPlayerPos = rig.transform.position;

            // Get distance between initial and current position coordinates
            float playerDist = Vector3.Distance(initPlayerPos, currPlayerPos);
            float leftDist = Vector3.Distance(initLeftPos, currLeftPos) - playerDist;
            float rightDist = Vector3.Distance(initRightPos, currRightPos) - playerDist;

            // Calculate how much the player moves forward
            if (leftOrRight)
                rig.transform.position += LeftRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
            else
                rig.transform.position += RightRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
            LeftRunningContainer.transform.position = rig.transform.position;
            RightRunningContainer.transform.position = rig.transform.position;

            leftOrRight = !leftOrRight;

            // Set current position coordinates as initial position coordinates
            initLeftPos = currLeftPos;
            initRightPos = currRightPos;
            initPlayerPos = currPlayerPos;
        }
    }
}
