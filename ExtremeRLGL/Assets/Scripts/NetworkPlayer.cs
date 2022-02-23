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
    private PhotonView photonView;

    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;

    private PlayerInteraction playerInteraction;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = GetComponent<PlayerInteraction>();
        photonView = GetComponent<PhotonView>();

        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);

        XROrigin rig = FindObjectOfType<XROrigin>();
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
        if (photonView.IsMine && playerInteraction.stopped)
        {
            // current fix to setting up camera/controller on scene change is to just keep finding them, so when scene changes, it will find them again
            // TODO: make it so that it doesn't have to do this everytime
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            XROrigin rig = FindObjectOfType<XROrigin>();
            if (rig != null)
            {
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
        }
    }

    public void MapPosition(Transform target, Transform rigTransform)
    {
        //target.position = rigTransform.position;
        //target.rotation = rigTransform.rotation;

        target.position = rigTransform.TransformPoint(trackingPositionOffset);
        target.rotation = rigTransform.rotation * Quaternion.Euler(trackingRotationOffset);

    }
}
