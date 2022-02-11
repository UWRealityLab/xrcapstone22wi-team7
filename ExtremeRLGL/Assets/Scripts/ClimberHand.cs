using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimberHand : MonoBehaviour
{
    // Variables
    public Transform floatingHand;
    public int touchCount;
    public bool isGrabbing;

    // OnCollisionEnter is called when this collider/rigidbody has begun touching another rigidbody/collider
    void OnCollisionEnter(Collision collision)
    {
        // Touch count increases if collider has the tag "ClimbingPoint"
        if (collision.collider.CompareTag("ClimbingPoint"))
        {
            touchCount++;
        }
    }

    // OnCollisionExit is called when this collider/rigidbody has stopped touching another rigidbody/collider
    void OnCollisionExit(Collision collision)
    {
        // Touch count decreases if collider has the tag "ClimbingPoint"
        if (collision.collider.CompareTag("ClimbingPoint"))
        {
            touchCount--;
        }
    }
}
