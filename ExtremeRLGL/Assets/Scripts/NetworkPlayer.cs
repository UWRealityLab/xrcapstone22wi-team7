using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class NetworkPlayer : MonoBehaviour
{
    public Transform body;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    public Transform climbingLeftHand;
    public Transform climbingRightHand;
    public Transform climbingLeftInteractor;
    public Transform climbingRightInteractor;
    private PhotonView photonView;

    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    XROrigin rig;
    

    private PlayerInteraction playerInteraction;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        photonView = GetComponent<PhotonView>();

        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        rig = FindObjectOfType<XROrigin>();
        //rig.transform.position = gameObject.transform.position;
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

        //DontDestroyOnLoad(rig);

        // Don't display the network avatar version of yourself
        if (photonView.IsMine)
        {
            foreach (var item in GetComponentsInChildren<Renderer>())
            {
                item.enabled = false;
            }
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Only update positions for your avatar
        if (photonView.IsMine && !playerInteraction.stopped)
        {
            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            rig = FindObjectOfType<XROrigin>();
            if (rig != null)
            {
                // make sure rig is same as gameObject's positiom (deals with wonky hand tracking before game starts)
                //rig.transform.position = gameObject.transform.position;

                headRig = rig.transform.Find("Camera Offset/Main Camera");
                leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
                rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");
            }
            // currently commented out so only movement scripts affect position/rotation
            // can uncomment head.rotation part if we want others to see direction player is looking at, but it does look weird at times (e.g. 180 rotations twist neck)
            head.rotation = headRig.rotation;
            //Vector3 bodyPos = new Vector3(headRig.position.x, 0, headRig.position.z);
            //body.position = bodyPos;
            //body.rotation = new Quaternion(0, headRig.rotation.y, 0, 1);
            MapPosition(leftHand, leftHandRig);
            MapPosition(rightHand, rightHandRig);
            //MapPosition(climbingLeftHand, leftHandRig);
            //MapPosition(climbingRightHand, rightHandRig);
            //MapPosition(climbingLeftInteractor, leftHandRig);
            //MapPosition(climbingRightInteractor, rightHandRig);
        }
    }

    public void MapPosition(Transform target, Transform rigTransform)
    {
        //target.position = rigTransform.position;
        //target.rotation = rigTransform.rotation;

        // this makes the arm tracking simpler - really only relies on controller rotation - but seems to prevent arms from getting stuck behind model
        trackingPositionOffset = gameObject.transform.position;

        target.position = rigTransform.TransformPoint(trackingPositionOffset);
        target.rotation = rigTransform.rotation * Quaternion.Euler(trackingRotationOffset);

    }
}
