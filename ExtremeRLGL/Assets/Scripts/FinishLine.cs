using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player hit the finishing line!");
        if (PhotonNetwork.IsMasterClient && GameManager.gameStage == GameStage.Playing && (other.gameObject.tag.Equals("Player") || other.gameObject.tag.Equals("Bot")))
        {
            // GameManager.gameManager.PlayerFinished(other.gameObject);
            Debug.Log("Player hit the finishing line!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
            GameManager.gameManager.GameEnd();
        }

            
    }
}
