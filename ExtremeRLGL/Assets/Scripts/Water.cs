using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    // Running and rowing movement scripts
    private RunningMovement runningMovement;
    private RowingMovement rowingMovement;

    // Start is called before the first frame update
    void Start()
    {
        // Get components for running and rowing movement scripts
        runningMovement = GetComponent<RunningMovement>();
        rowingMovement = GetComponent<RowingMovement>();
    }

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    private void OnCollisionEnter(Collision collision)
    {
        // Executes if this collider touches an object with the "water" tag
        if (collision.gameObject.CompareTag("Water"))
        {
            // GameObject.FindGameObjectWithTag("Water").GetComponent<Renderer>().material.color = Color.green;
            runningMovement.enabled = false;
            rowingMovement.enabled = true;
        }
    }

    // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider
    private void OnCollisionExit(Collision collision)
    {
        // Executes if this collider touches an object with the "water" tag
        if (collision.gameObject.CompareTag("Water")) 
        {
            // GameObject.FindGameObjectWithTag("Water").GetComponent<Renderer>().material.color = Color.red;
            runningMovement.enabled = true;
            rowingMovement.enabled = false;
        }
    }
}
