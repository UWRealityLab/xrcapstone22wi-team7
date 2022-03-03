using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using Photon.Pun;

public class Teleport : MonoBehaviour
{
    private PhotonView photonView;
    private bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            ToStartLine();
            initialized = true;
        }
    }

    public void ToStartLine()
    {
        if (photonView.IsMine)
        {
            StartLine startLineScript = GameObject.FindGameObjectWithTag("StartLine").GetComponent<StartLine>();
            XROrigin rig = FindObjectOfType<XROrigin>();
            Vector3 position = startLineScript.GetNextPos();
            gameObject.transform.position = position;
        }
    }
}
