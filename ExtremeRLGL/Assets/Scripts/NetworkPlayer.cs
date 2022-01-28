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

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        XROrigin rig = FindObjectOfType<XROrigin>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");

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
        if (photonView.IsMine)
        {
            // Only update positions for your avatar
            head.rotation = headRig.rotation;
            Vector3 bodyPos = new Vector3(headRig.position.x, 0, headRig.position.z);
            body.position = bodyPos;
            body.rotation = new Quaternion(0, headRig.rotation.y, 0, 1);
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
