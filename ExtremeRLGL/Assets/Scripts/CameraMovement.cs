using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using Photon.Pun;

public class CameraMovement : MonoBehaviour
{
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            // Move XROrigin based on player position
            XROrigin rig = FindObjectOfType<XROrigin>();
            rig.transform.position = gameObject.transform.position;
            rig.transform.rotation = new Quaternion(rig.transform.rotation.x, gameObject.transform.rotation.y, gameObject.transform.rotation.z, 1);

            MenuButtonReactor.miniMenu.transform.position = gameObject.transform.position + new Vector3(0f, 1.4f, 2.4f);
        }
    }
}
