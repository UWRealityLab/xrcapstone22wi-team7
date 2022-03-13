using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using Photon.Pun;

public class Teleport : MonoBehaviour
{
    private PhotonView photonView;
    private bool initialized = false;
    private int id;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        id = GameObject.FindGameObjectsWithTag("Player").Length - 1;
    }

    // Update is called once per frame
    void Update()
    {   
        
        if (photonView.IsMine && !initialized)
        {
            ToStartLine();
            initialized = true;
        }
        
    }

    public void ToStartLine()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Set my player to startline!!!!");
            StartLine startLineScript = GameObject.FindGameObjectWithTag("StartLine").GetComponent<StartLine>();
            XROrigin rig = FindObjectOfType<XROrigin>();
            rig.GetComponent<ClimbingMovement>().Reset();
            Vector3 position = startLineScript.GetNextPos();
            gameObject.transform.position = new Vector3(position.x, position.y, position.z);
            rig.transform.position = new Vector3(position.x, position.y, position.z);
            
        }
    }
}
