using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionDetection : MonoBehaviour
{
    // Headset and controller game objects
    public GameObject MainCamera;
    public GameObject LeftHand;
    public GameObject RightHand;

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

    // Start is called before the first frame update
    void Start()
    {
        if (Time.timeSinceLevelLoad > 1f)
        {
            // Get initial position coordinates
            initCameraPos = MainCamera.transform.position;
            initLeftPos = LeftHand.transform.position;
            initRightPos = RightHand.transform.position;

            // Get initial rotation coordinates
            initCameraRot = MainCamera.transform.rotation.eulerAngles;
            initLeftRot = LeftHand.transform.rotation.eulerAngles;
            initRightRot = RightHand.transform.rotation.eulerAngles;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Get current position coordinates
        currCameraPos = MainCamera.transform.position;
        currLeftPos = LeftHand.transform.position;
        currRightPos = RightHand.transform.position;

        // Get current rotation coordinates
        currCameraRot = MainCamera.transform.rotation.eulerAngles;
        currLeftRot = LeftHand.transform.rotation.eulerAngles;
        currRightRot = RightHand.transform.rotation.eulerAngles;

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
        }

        // Executes if calculated distances are less than their respective thresholds
        else
        {
            Debug.Log("Freeze");
            // GetComponent<Renderer>().material.color = Color.red;
        }
    }
}