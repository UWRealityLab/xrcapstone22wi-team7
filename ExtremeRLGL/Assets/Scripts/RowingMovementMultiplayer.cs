using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

public class RowingMovementMultiplayer : MonoBehaviour
{

    private PhotonView photonView;
    private PlayerInteraction playerInteraction;

    // Headset, controller, and container game objects
    public Transform LeftHand;
    public Transform RightHand;
    public Transform MainCamera;
    public GameObject LeftRowingContainer;
    public GameObject RightRowingContainer;

    // Initial position coordinates
    private Vector3 initLeftPos;
    private Vector3 initRightPos;
    private Vector3 initPlayerPos;

    // Current position coordinates
    private Vector3 currLeftPos;
    private Vector3 currRightPos;
    private Vector3 currPlayerPos;

    // Speed variable to determine how far the player moves forward
    public float speed = 120;

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

        playerInteraction = GetComponent<PlayerInteraction>();

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
            // LeftRunningContainer = gameObject.transform.Find("LeftRunning").gameObject;
            // RightRunningContainer = gameObject.transform.Find("RightRunning").gameObject;
            Debug.Log("LeftContainer:");
            Debug.Log(LeftRowingContainer);
            Debug.Log("RightContainer:");
            Debug.Log(RightRowingContainer);
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
        }

        if (photonView.IsMine && !playerInteraction.stopped && LeftRowingContainer != null && GameManager.gameStage == GameStage.Playing)
        {
            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement


            // Rotate the container depending on the rotation of the left controller
            LeftRowingContainer.transform.eulerAngles = new Vector3(LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y, 0);
            RightRowingContainer.transform.eulerAngles = new Vector3(RightHand.transform.eulerAngles.x, RightHand.transform.eulerAngles.y, 0);

            // Get current position coordinates
            currLeftPos = LeftHand.position;
            currRightPos = RightHand.position;
            currPlayerPos = rig.transform.position;

            // Get distance between initial and current position x coordinates
            float playerDistX = currPlayerPos.x - initPlayerPos.x;
            float leftDistX = currLeftPos.x - initLeftPos.x - playerDistX;
            float rightDistX = currRightPos.x - initRightPos.x - playerDistX;

            // Get distance between initial and current position y coordinates
            float playerDistY = currPlayerPos.y - initPlayerPos.y;
            float leftDistY = currLeftPos.y - initLeftPos.y - playerDistY;
            float rightDistY = currRightPos.y - initRightPos.y - playerDistY;

            // Calculate how much the player moves forward
            if (leftOrRight)
                if (leftDistX < 0 && rightDistX < 0)
                {
                    gameObject.transform.position -= LeftRowingContainer.transform.forward * (leftDistX + rightDistX) * speed * Time.deltaTime;
                }
            if (leftDistY < 0 && rightDistY < 0)
            {
                gameObject.transform.position -= LeftRowingContainer.transform.forward * (leftDistY + rightDistY) * speed * Time.deltaTime;
            }
            else
            if (leftDistX < 0 && rightDistX < 0)
            {
                gameObject.transform.position -= RightRowingContainer.transform.forward * (leftDistX + rightDistX) * speed * Time.deltaTime;
            }
            if (leftDistY < 0 && rightDistY < 0)
            {
                gameObject.transform.position -= RightRowingContainer.transform.forward * (leftDistY + rightDistY) * speed * Time.deltaTime;
            }
            rig.transform.position = gameObject.transform.position;

            leftOrRight = !leftOrRight;

            // Set current position coordinates as initial position coordinates
            initLeftPos = currLeftPos;
            initRightPos = currRightPos;
            initPlayerPos = currPlayerPos;
        }
        else
        {
            initPlayerPos = rig.transform.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;
        }
    }
}