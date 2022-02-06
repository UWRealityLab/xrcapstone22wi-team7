using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;


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

    // Animator
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();
        MainCamera = rig.transform.Find("Camera Offset/Main Camera");
        LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        RightHand = rig.transform.Find("Camera Offset/RightHand Controller");

        if (photonView.IsMine && Time.timeSinceLevelLoad > 1f)
        {
            // Get initial position coordinates
            initCameraPos = MainCamera.position;
            initLeftPos = LeftHand.position;
            initRightPos = RightHand.position;

            // Get initial rotation coordinates
            initCameraRot = MainCamera.rotation.eulerAngles;
            initLeftRot = LeftHand.rotation.eulerAngles;
            initRightRot = RightHand.rotation.eulerAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
        // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
        XROrigin rig = FindObjectOfType<XROrigin>();
        MainCamera = rig.transform.Find("Camera Offset/Main Camera");
        LeftHand = rig.transform.Find("Camera Offset/LeftHand Controller");
        RightHand = rig.transform.Find("Camera Offset/RightHand Controller");

        if (photonView.IsMine)
        {
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
                // Set current position coordinates as initial position coordinates
                initCameraPos = currCameraPos;
                initLeftPos = currLeftPos;
                initRightPos = currRightPos;

                // Set current rotation coordinates as initial rotation coordinates 
                initCameraRot = currCameraRot;
                initLeftRot = currLeftRot;
                initRightRot = currRightRot;
                Debug.Log("Move");
                // GetComponent<Renderer>().material.color = Color.green;

                animator.SetBool("isMoving", true);
            }

            // Executes if calculated distances are less than their respective thresholds
            else
            {
                Debug.Log("Freeze");
                // GetComponent<Renderer>().material.color = Color.red;

                animator.SetBool("isMoving", false);
            }
        }
    }
}