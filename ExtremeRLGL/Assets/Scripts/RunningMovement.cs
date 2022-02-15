using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunningMovement : MonoBehaviour
{
    // Headset, controller, and container game objects
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject MainCamera;
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
    private bool leftOrRight = false;

    // Start is called before the first frame update
    void Start()
    {
        // Get initial position coordinates
        initPlayerPos = transform.position;
        initLeftPos = LeftHand.transform.position;
        initRightPos = RightHand.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate the container depending on the rotation of the left controller
        LeftRunningContainer.transform.eulerAngles = new Vector3(0, LeftHand.transform.eulerAngles.y, 0);
        RightRunningContainer.transform.eulerAngles = new Vector3(0, RightHand.transform.eulerAngles.y, 0);

        // Get current position coordinates
        currLeftPos = LeftHand.transform.position;
        currRightPos = RightHand.transform.position;
        currPlayerPos = transform.position;

        // Get distance between initial and current position coordinates
        float playerDist = Vector3.Distance(initPlayerPos, currPlayerPos);
        float leftDist = Vector3.Distance(initLeftPos, currLeftPos) - playerDist;
        float rightDist = Vector3.Distance(initRightPos, currRightPos) - playerDist;
  
        // Calculate how much the player moves forward
        if (leftOrRight)
            transform.position += LeftRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;
        else
            transform.position += RightRunningContainer.transform.forward * (leftDist + rightDist) * speed * Time.deltaTime;

        leftOrRight = !leftOrRight;

        // Set current position coordinates as initial position coordinates
        initLeftPos = currLeftPos;
        initRightPos = currRightPos;
        initPlayerPos = currPlayerPos;
    }
}
