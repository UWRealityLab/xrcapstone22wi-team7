using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Checkpoint : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameStage != GameStage.Playing)
        {
            GetComponent<Renderer>().enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine) 
        {
            other.GetComponent<Teleport>().checkpoint = GetComponent<Collider>();
            GetComponent<Renderer>().enabled = false;
            Debug.Log("Hit checkpoint!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
    }
}
