using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using Photon.Pun;

public class MiniMenuMovement : MonoBehaviour
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
            // Move menu to player
            MenuButtonReactor.miniMenu.transform.position = gameObject.transform.position + new Vector3(0f, 1.4f, 2.4f);

            GameObject.Find("Canvas").transform.position = gameObject.transform.position + new Vector3(0f, 1.4f, 2.4f);
        }
    }
}
