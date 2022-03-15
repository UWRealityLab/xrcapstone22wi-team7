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
    public Collider checkpoint;
    private GameObject startline;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        id = GameObject.FindGameObjectsWithTag("Player").Length - 1;
        startline = GameObject.FindGameObjectWithTag("StartLine");
        checkpoint = startline.GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            if (startline == null)
            {
                startline = GameObject.FindGameObjectWithTag("StartLine");
                checkpoint = startline.GetComponent<Collider>();
            }

            if (!initialized)
            {
                ToStartLine();
                
            }

            if (GameManager.gameStage != GameStage.Playing)
            {
                checkpoint = startline.GetComponent<Collider>();
            }
        }
    }

    public void ToStartLine()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Set my player to startline!!!!");
            StartLine startLineScript = startline.GetComponent<StartLine>();
            XROrigin rig = FindObjectOfType<XROrigin>();
            rig.GetComponent<ClimbingMovement>().Reset();
            Vector3 position = startLineScript.GetNextPos();
            gameObject.transform.position = new Vector3(position.x, position.y, position.z);
            rig.transform.position = new Vector3(position.x, position.y, position.z);
            initialized = true;
        }
    }

    public void ToCheckpoint()
    {
        if (photonView.IsMine)
        {
            Debug.Log("Set my player to last checkpoint!!!!");
            Vector3 position = new Vector3(
                Random.Range(checkpoint.bounds.min.x, checkpoint.bounds.max.x),
                Random.Range(checkpoint.bounds.min.y, checkpoint.bounds.max.y),
                Random.Range(checkpoint.bounds.min.z, checkpoint.bounds.max.z)
            );
            GetComponent<WaterMultiplayer>().EndRowing();
            XROrigin rig = FindObjectOfType<XROrigin>();
            rig.GetComponent<ClimbingMovement>().Reset();
            gameObject.transform.position = new Vector3(position.x, position.y + 0.5f, position.z);
            // rig.transform.position = new Vector3(position.x, position.y + 1, position.z);
        }
    }
}
