using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StartLine : MonoBehaviour, IPunObservable
{
    private int length;
    private int currentIdx = 4;
    private int coordinateIdx;
    public float playerWidth;

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


    // Start is called before the first frame update
    void Start()
    {
        Vector3 size = gameObject.GetComponent<Collider>().bounds.size;

        float longest = -1;
        for (int i = 0; i < 3; i++)
        {
            Debug.Log("size" + i + " :" + size[i]);
            if (size[i] > longest)
            {
                Debug.Log("Bigger");
                longest = size[i];
                coordinateIdx = i;
            }
        }
        Debug.Log("Longest length: " + longest);
        Debug.Log("Player width: " + playerWidth);
        length = (int)(longest / playerWidth);
        Debug.Log("Startline spaces " + length);
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
        pos[1] = pos[1] + 2.0f;
        currentIdx++;
        Debug.Log(pos);
        return pos;
    }
}
