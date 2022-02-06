using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using Unity.XR.CoreUtils;

public class RunningMovementMultiplayer : MonoBehaviour
{

    private PhotonView photonView;
    private NetworkPlayer networkPlayer;

    // Headset, controller, and container game objects
    public Transform LeftHand;
    public Transform RightHand;
    public Transform MainCamera;
    public GameObject RunningContainer;

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

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && !networkPlayer.stopped)
        {
            // Rotate the container depending on the rotation of the left controller
            RunningContainer.transform.eulerAngles = new Vector3(0, LeftHand.transform.eulerAngles.y, 0);

            // Get current position coordinates
            currLeftPos = LeftHand.position;
            currRightPos = RightHand.position;
            currPlayerPos = rig.transform.position;

            // Get distance between initial and current position coordinates
            float playerDist = Vector3.Distance(initPlayerPos, currPlayerPos);
            float leftDist = Vector3.Distance(initLeftPos, currLeftPos) - playerDist;
            float rightDist = Vector3.Distance(initRightPos, currRightPos) - playerDist;

            // Calculate how much the player moves forward
            rig.transform.position += RunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
            RunningContainer.transform.position = rig.transform.position;

            // Set current position coordinates as initial position coordinates
            initLeftPos = currLeftPos;
            initRightPos = currRightPos;
            initPlayerPos = currPlayerPos;
        }
    }
}
