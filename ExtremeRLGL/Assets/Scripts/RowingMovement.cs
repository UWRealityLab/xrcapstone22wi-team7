using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowingMovement : MonoBehaviour
{
    // Headset, controller, and container game objects
    public GameObject LeftHand;
    public GameObject RightHand;
    public GameObject MainCamera;
    public GameObject RowingContainer;

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
        RowingContainer.transform.eulerAngles = new Vector3(LeftHand.transform.eulerAngles.x, LeftHand.transform.eulerAngles.y, 0);

        // Get current position coordinates
        currLeftPos = LeftHand.transform.position;
        currRightPos = RightHand.transform.position;
        currPlayerPos = transform.position;

        // Get distance between initial and current position x coordinates
        float playerDistX = currPlayerPos.x - initPlayerPos.x;
        float leftDistX = currLeftPos.x - initLeftPos.x - playerDistX;
        float rightDistX = currRightPos.x - initRightPos.x - playerDistX;

        if (leftDistX < 0 && rightDistX < 0)
        {
            // Calculate how much the player moves forward
            transform.position -= RowingContainer.transform.forward * (leftDistX + rightDistX) * speed * Time.deltaTime;
        }

        // Get distance between initial and current position y coordinates
        float playerDistY = currPlayerPos.y - initPlayerPos.y;
        float leftDistY = currLeftPos.y - initLeftPos.y - playerDistY;
        float rightDistY = currRightPos.y - initRightPos.y - playerDistY;

        if (leftDistY < 0 && rightDistY < 0)
        {
            // Calculate how much the player moves forward
            transform.position -= RowingContainer.transform.forward * (leftDistY + rightDistY) * speed * Time.deltaTime;
        }

        // Set current position coordinates as initial position coordinates
        initLeftPos = currLeftPos;
        initRightPos = currRightPos;
        initPlayerPos = currPlayerPos;
    }
}