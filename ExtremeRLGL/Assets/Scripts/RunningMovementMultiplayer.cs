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
    private PlayerInteraction playerInteraction;
    private ClimbingMovement climbingMovement;
    private PlayerPowerup playerPowerup;

    // Headset, controller, and container game objects
    public Transform LeftHand;
    public Transform RightHand;
    public Transform MainCamera;
    public GameObject LeftRunningContainer;
    public GameObject RightRunningContainer;

    // For moving up slopes
    public RaycastHit slopeHit;
    public int speedUpScale;

    // For jumping movement
    public float jumpThreshold;
    public float jumpAmount;
    public LayerMask ground;
    public Transform groundCheck;

    // Initial position coordinates
    private Vector3 initLeftPos;
    private Vector3 initRightPos;
    private Vector3 initPlayerPos; 

    // Current position coordinates
    private Vector3 currLeftPos;
    private Vector3 currRightPos;
    private Vector3 currPlayerPos;

    // Headset InputDevice and position coordinates for jumping movement
    InputDevice headset;
    private Vector3 initHeadsetPos;
    private Vector3 currHeadsetPos;

    // Speed variable to determine how far the player moves forward
    public float speed = 80;

    private XROrigin rig;
    private bool leftOrRight = false;


    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        rig = FindObjectOfType<XROrigin>();
        if (rig != null)
        {
            MainCamera = rig.transform.Find("Camera Offset/Main Camera");
            LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
            RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
        }

        headset = InputDevices.GetDeviceAtXRNode(XRNode.Head);

        playerInteraction = GetComponent<PlayerInteraction>();
        climbingMovement = GetComponent<ClimbingMovement>();
        playerPowerup = GetComponent<PlayerPowerup>();

        if (photonView.IsMine) 
        { 
            // Get initial position coordinates
            initPlayerPos = rig.transform.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;
            headset.TryGetFeatureValue(CommonUsages.devicePosition, out initHeadsetPos);
        }
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        rig = FindObjectOfType<XROrigin>();
        if (photonView.IsMine && rig != null)
        {
            MainCamera = rig.transform.Find("Camera Offset/Main Camera");
            LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
            RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
            rig.GetComponent<ClimbingMovement>().player = gameObject;
            initPlayerPos = rig.transform.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;
            Debug.Log("Running initial position reset!");
        }
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
            // LeftRunningContainer = gameObject.transform.Find("LeftRunning").gameObject;
            // RightRunningContainer = gameObject.transform.Find("RightRunning").gameObject;
            Debug.Log("LeftContainer:");
            Debug.Log(LeftRunningContainer);
            Debug.Log("RightContainer:");
            Debug.Log(RightRunningContainer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        rig = FindObjectOfType<XROrigin>();
        if (rig != null)
        {
            MainCamera = rig.transform.Find("Camera Offset/Main Camera");
            LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
            RightHand = rig.transform.Find("Camera Offset/RightHand Controller");
            if (photonView.IsMine) 
                rig.GetComponent<ClimbingMovement>().player = gameObject;
        }

        if (photonView.IsMine && !playerInteraction.stopped && LeftRunningContainer != null && GameManager.gameStage == GameStage.Playing)
        {
            // Powerup: SpeedUp
            float currentSpeed;
            if (playerPowerup.isActivate(PowerUpType.SPEED_UP))
                currentSpeed = speed * speedUpScale;
            else
                currentSpeed = speed;

            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            

            // Rotate the container depending on the rotation of the left controller
            LeftRunningContainer.transform.eulerAngles = new Vector3(0, LeftHand.transform.eulerAngles.y, 0);
            RightRunningContainer.transform.eulerAngles = new Vector3(0, RightHand.transform.eulerAngles.y, 0);

            // Get current position coordinates
            currLeftPos = LeftHand.position;
            currRightPos = RightHand.position;
            currPlayerPos = rig.transform.position;

            // Get current headset coordinates (for jumping movement)
            headset.TryGetFeatureValue(CommonUsages.devicePosition, out currHeadsetPos);

            // Get distance between initial and current position coordinates
            float playerDist = Vector3.Distance(initPlayerPos, currPlayerPos);
            float leftDist = Vector3.Distance(initLeftPos, currLeftPos) - playerDist;
            float rightDist = Vector3.Distance(initRightPos, currRightPos) - playerDist;

            // Calculate how much the player moves forward

            /*
            if (leftOrRight)
                rig.transform.position += LeftRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
            else
                rig.transform.position += RightRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
            gameObject.transform.position = rig.transform.position;
            */

            // check if player is jumping and if they can jump
            if (isGrounded() && currHeadsetPos.y - initHeadsetPos.y > jumpThreshold && initHeadsetPos.y != 0)
            {
                gameObject.transform.position += new Vector3(0, jumpAmount, 0);
            }

            if (!OnSlope())
            {
                if (leftOrRight)
                    gameObject.transform.position += LeftRunningContainer.transform.forward * (leftDist + rightDist) * currentSpeed * Time.deltaTime;
                else
                    gameObject.transform.position += RightRunningContainer.transform.forward * (leftDist + rightDist) * currentSpeed * Time.deltaTime;
            }
            else
            {
                if (leftOrRight)
                    gameObject.transform.position += Vector3.ProjectOnPlane(LeftRunningContainer.transform.forward, slopeHit.normal) * (leftDist + rightDist) * currentSpeed * Time.deltaTime;
                else
                    gameObject.transform.position += Vector3.ProjectOnPlane(RightRunningContainer.transform.forward, slopeHit.normal) * (leftDist + rightDist) * currentSpeed * Time.deltaTime;
            }
            rig.transform.position = gameObject.transform.position;

            leftOrRight = !leftOrRight;

            // Set current position coordinates as initial position coordinates
            initLeftPos = currLeftPos;
            initRightPos = currRightPos;
            initPlayerPos = currPlayerPos;
            initHeadsetPos = currHeadsetPos;
        }
        else 
        {
            initPlayerPos = rig.transform.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;
            initHeadsetPos = currHeadsetPos;
        }
    }


    public bool OnSlope()
    {
        // Check if player is on a slope
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1.6f))
        {
            if (slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    bool isGrounded()
    {
        return Physics.CheckSphere(groundCheck.position, 0.05f, ground);
    }
}
