using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLine : MonoBehaviour
{
    private int coordinateIdx;
    private float longest;
    private PhotonView photonView;

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            Debug.Log("Sent currentIdx = " + currentIdx);
            stream.SendNext(currentIdx);
        }
        else
        {
            // Network player, receive data
            currentIdx = (int)stream.ReceiveNext();
            Debug.Log("Recieved currentIdx = " + currentIdx);
        }
    }
    */


    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;

        longest = -1;
        for (int i = 0; i < 3; i++)
        {
            if (size[i] > longest)
            {
                longest = size[i];
                coordinateIdx = i;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Vector3 GetNextPos()
    {
        Vector3 pos = gameObject.GetComponent<Collider>().bounds.min;
        if (!PhotonNetwork.IsMasterClient)
            pos[coordinateIdx] = pos[coordinateIdx] + NextFloat(0, longest);
        else
            pos[coordinateIdx] = pos[coordinateIdx] + longest / 2;
        pos[1] = pos[1] + 0.5f;
        return pos;
    }

    private float NextFloat(float min, float max)
    {
        System.Random random = new System.Random();
        double val = (random.NextDouble() * (max - min) + min);
        return (float)val;
    }

    /*
    [PunRPC]
    public void updateIdx()
    {
        currentIdx++;
        Debug.Log("Index updated into" + currentIdx);
    }
    */

}
