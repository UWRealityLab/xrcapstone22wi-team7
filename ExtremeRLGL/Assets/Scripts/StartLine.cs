using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLine : MonoBehaviour
{
    private int length;
    private int currentIdx = 4;
    private int coordinateIdx;
    public float playerWidth;
    private PhotonView photonView;

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(currentIdx);
        }
        else
        {
            // Network player, receive data
            currentIdx = (int)stream.ReceiveNext();
        }
    }
    */


    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;

        float longest = -1;
        for (int i = 0; i < 3; i++)
        {
            if (size[i] > longest)
            {
                longest = size[i];
                coordinateIdx = i;
            }
        }
        length = (int)(longest / playerWidth);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetNextPos()
    {
        Debug.Log("Current startLine Index:" + currentIdx);
        Vector3 pos = gameObject.GetComponent<Collider>().bounds.min;
        pos[coordinateIdx] = pos[coordinateIdx] + playerWidth * (currentIdx % length);
        pos[1] = pos[1] + 0.5f;
        currentIdx++;
        photonView.RPC("updateIdx", RpcTarget.All, currentIdx);
        Debug.Log(pos);
        return pos;
    }

    [PunRPC]
    public void updateIdx(int newIdx)
    {
        currentIdx = newIdx;
    }
}
