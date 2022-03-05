using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class HandPhysicalMovement : MonoBehaviour
{
    // Tracking status of position/roation & the rigidbody
    public bool trackPos = true;
    public bool trackRot = true;
    public Rigidbody rigidBody = null;
    public ClimbingMovement climbingMovement;

    // Serialized fields so variables can show up in the editor
    [SerializeField, Range(0f, 10f)]
    float velocityDamping = 1f;
    [SerializeField, Range(0f, 10f)]
    float angVelocityDamping = 1f;
    [SerializeField, Range(0f, 10f)]
    float velocityScale = 0.5f;
    [SerializeField, Range(0f, 10f)]
    float angVelocityScale = 0.5f;

    // Transform variables
    public Transform m_TargetToMove;
    public Transform targetAttach;
    public Transform m_TargetAttachPointTransform;

    // Hand-related variables
    public HandPhysicalMovement otherHand = null;
    public Vector3 offset = new Vector3();
    public bool isLeftHand = false;


    // Start is called before the first frame update
    void Start()
    {
        rigidBody = m_TargetToMove.GetComponent<Rigidbody>();
        targetAttach = transform;
    }

    // FixedUpdate is called every fixed frame-rate frame
    void FixedUpdate(){
        updateHand(Time.deltaTime);
        reduceJitter();
    }

    // updateHand is called every fixed frame-rate frame
    void updateHand(float deltaTime){
        // Only apply velocity when we are actually climbing
        //if (climbingMovement.Climbing)
        //{

            // Executes if the position is being tracked
            if (trackPos)
            {
                rigidBody.velocity *= (1f - velocityDamping);
                var deltaPos = targetAttach.position - m_TargetToMove.position + Vector3.Scale(m_TargetToMove.forward, offset);
                var velocity = deltaPos / deltaTime;
                if (!float.IsNaN(velocity.x))
                    rigidBody.velocity += (velocity * velocityScale);
            }

            // Executes if the rotation is being tracked
            if (trackRot)
            {
                rigidBody.angularVelocity *= (1f - angVelocityDamping);
                var deltaRot = targetAttach.rotation * Quaternion.Inverse(m_TargetToMove.rotation);
                deltaRot.ToAngleAxis(out var angle, out var axis);
                if (angle > 180f)
                    angle -= 360f;
                if (Mathf.Abs(angle) > Mathf.Epsilon)
                {
                    var angVel = (axis * (angle * Mathf.Deg2Rad)) / deltaTime;
                    if (!float.IsNaN(angVel.x))
                        rigidBody.angularVelocity += (angVel * angVelocityScale);
                }
            }
        //}
    }

    // reduceJitter is called every fixed frame-rate frame
    void reduceJitter(){
        // Executes if the target to move has a FixedJoint component
        if (m_TargetToMove.GetComponent<FixedJoint>())
        {
            // Executes if the other hand's target to move has a FixedJoint component
            if (otherHand.m_TargetToMove.GetComponent<FixedJoint>())
            {
                float handDist = Vector3.Distance(m_TargetToMove.transform.position, otherHand.m_TargetToMove.transform.position);
                float controllerDist = Vector3.Distance(transform.position, otherHand.transform.position);
                float distRatio = Mathf.Pow(controllerDist / handDist, 2);
                velocityScale = Mathf.Clamp((distRatio >= 1 ? 1f / distRatio : 1f * distRatio), 0.0f, 1f);
                angVelocityScale = Mathf.Clamp((distRatio >= 1 ? 1f / distRatio : 1f * distRatio), 0.0f, 1f);
            }

            // Executes if the other hand's target to move doesn't have a FixedJoint component
            else
            {
                velocityScale = Mathf.Clamp(1f - Vector3.Distance(transform.position, m_TargetToMove.position), 0.1f, 1f);
                angVelocityScale = Mathf.Clamp(1f - Vector3.Distance(transform.position, m_TargetToMove.position), 0.1f, 1f);
            }
        }

        // Executes if the target to move doesn't have a FixedJoint component
        else
        {
            velocityScale = 1f;
            angVelocityScale = 1f;
        }
    }
}