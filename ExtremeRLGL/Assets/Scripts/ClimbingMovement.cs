using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;
using Unity.XR.CoreUtils;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingMovement : MonoBehaviour
{
    // Controllers and buttons
    public ClimberHand RightHand;
    public ClimberHand LeftHand;
    public InputActionProperty rightButton;
    public InputActionProperty leftButton;

    // Other public variables
    public ConfigurableJoint ClimbingContainer;
    public bool Climbing;
    public ClimberHand ActiveHand;
    public string handSide = null;
    public Rigidbody movedRigidbody;
    public bool eachHandHolds = false;
    public GameObject player;

    // Private variables
    private RunningMovementMultiplayer runningMovement;
    private CapsuleCollider capsule = null;
    private Vector3 offset = Vector3.zero;
    

    // onEnable is called when the object becomes enabled and active
    void OnEnable()
    {
        // Executes if the left or right buttons are pressed
        if (rightButton.action != null) rightButton.action.Enable();
        if (leftButton.action != null) leftButton.action.Enable();
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Get RigidBody, CapsulCollider, and RunningMovement components
        movedRigidbody = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
                runningMovement = player.GetComponent<RunningMovementMultiplayer>();
        }
    }


    // FixedUpdate is called every fixed frame-rate frame
    void FixedUpdate()
    {
        if (runningMovement == null)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PhotonView>().IsMine)
                    runningMovement = player.GetComponent<RunningMovementMultiplayer>();
            }
        }

        if (Climbing)
        {
            // Calculates the target position of the climbing container based on the ActiveHand, floatingHand, and offset variables
            var activeHandBuff = ClimbingContainer.transform.position - ActiveHand.floatingHand.position + ActiveHand.floatingHand.forward * -offset.magnitude;
            ClimbingContainer.targetPosition = -((ActiveHand.transform.position + activeHandBuff) - transform.position);
            player.transform.position = gameObject.transform.position;
        }
    }

    // handClimbGrab is called when the user is grabbing an object with their hand(s)
    public void handClimbGrab(ClimberHand climbingHand)
    {
        // Executes if the interactor has the "ClimbingPoint" tag
        if (climbingHand.gameObject.GetComponent<XRDirectInteractor>().firstInteractableSelected.transform.CompareTag("ClimbingPoint"))
        {
            ActiveHand = climbingHand;
            Climbing = true;
            ClimbingContainer.transform.position = climbingHand.transform.position;
            offset = climbingHand.GetComponent<HandPhysicalMovement>().offset;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            climbingHand.isGrabbing = true;
            runningMovement.enabled = false;
        }
    }

    // handClimbRelease is called when the user is not grabbing an object with their hand(s)
    public void handClimbRelease(ClimberHand climbingHand)
    {
        // Executes if only the left hand is grabbing an object
        if (climbingHand == RightHand && LeftHand.isGrabbing)
        {
            ActiveHand = LeftHand;
            Climbing = true;
            ClimbingContainer.transform.position = LeftHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            RightHand.isGrabbing = false;
            LeftHand.isGrabbing = true;
            runningMovement.enabled = false;
            return;
        }

        // Executes if only the right hand is grabbing an object
        if (climbingHand == LeftHand && RightHand.isGrabbing)
        {
            ActiveHand = RightHand;
            Climbing = true;
            ClimbingContainer.transform.position = RightHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            RightHand.isGrabbing = true;
            LeftHand.isGrabbing = false;
            runningMovement.enabled = false;
            return;
        }

        // Executes if neither hands are grabbing an object
        ClimbingContainer.connectedBody = null;
        Climbing = false;
        movedRigidbody.useGravity = true;
        ActiveHand = null;
        handSide = null;
        RightHand.isGrabbing = false;
        LeftHand.isGrabbing = false;
        runningMovement.enabled = true;
        eachHandHolds = false;
    }

    // updateHand is called when the user presses/releases a button (i.e. is grabbing/releasing an object)
    public void updateHand(ClimberHand climbingHand)
    {
        if (rightButton.action.ReadValue<float>() == 1 && RightHand.touchCount != 0)
        {
            ActiveHand = RightHand;
            Climbing = true;
            ClimbingContainer.transform.position = RightHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            RightHand.isGrabbing = true;
            runningMovement.enabled = false;
        }
        if (rightButton.action.ReadValue<float>() == 1 && leftButton.action.ReadValue<float>() == 0 && RightHand.isGrabbing)
        {
            ActiveHand = RightHand;
            Climbing = true;
            ClimbingContainer.transform.position = RightHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            RightHand.isGrabbing = true;
            LeftHand.isGrabbing = false;
            runningMovement.enabled = false;
            eachHandHolds = false;
        }
        if (leftButton.action.ReadValue<float>() == 1 && LeftHand.touchCount != 0)
        {
            ActiveHand = LeftHand;
            Climbing = true;
            ClimbingContainer.transform.position = LeftHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            LeftHand.isGrabbing = true;
            runningMovement.enabled = false;
        }
        if (rightButton.action.ReadValue<float>() == 0 && leftButton.action.ReadValue<float>() == 1 && LeftHand.isGrabbing)
        {
            ActiveHand = LeftHand;
            Climbing = true;
            ClimbingContainer.transform.position = LeftHand.transform.position;
            movedRigidbody.useGravity = false;
            ClimbingContainer.connectedBody = movedRigidbody;
            RightHand.isGrabbing = false;
            LeftHand.isGrabbing = true; ;
            runningMovement.enabled = false;
            eachHandHolds = false;
        }
        if (leftButton.action.ReadValue<float>() == 0 && rightButton.action.ReadValue<float>() == 1 && LeftHand.isGrabbing && eachHandHolds)
        {
            ClimbingContainer.connectedBody = null;
            Climbing = false;
            movedRigidbody.useGravity = true;
            ActiveHand = null;
            handSide = null;
            RightHand.isGrabbing = false;
            LeftHand.isGrabbing = false;
            runningMovement.enabled = true;
        }
        if (rightButton.action.ReadValue<float>() == 0 && leftButton.action.ReadValue<float>() == 1 && RightHand.isGrabbing && eachHandHolds)
        {
            ClimbingContainer.connectedBody = null;
            Climbing = false;
            movedRigidbody.useGravity = true;
            ActiveHand = null;
            handSide = null;
            RightHand.isGrabbing = false;
            LeftHand.isGrabbing = false;
            runningMovement.enabled = true;
        }
        if (rightButton.action.ReadValue<float>() == 1 && leftButton.action.ReadValue<float>() == 1)
        {
            eachHandHolds = true;
        }
        if (rightButton.action.ReadValue<float>() == 0 && leftButton.action.ReadValue<float>() == 0)
        {
            ClimbingContainer.connectedBody = null;
            Climbing = false;
            movedRigidbody.useGravity = true;
            ActiveHand = null;
            handSide = null;
            RightHand.isGrabbing = false;
            LeftHand.isGrabbing = false;
            runningMovement.enabled = true;
            eachHandHolds = false;
        }
    }
}