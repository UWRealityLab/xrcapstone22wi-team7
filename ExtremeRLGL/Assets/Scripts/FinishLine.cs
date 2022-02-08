using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FinishLine : MonoBehaviour
{
    public delegate void SceneLoaded();
    public static event SceneLoaded OnSceneLoaded;

    // Start is called before the first frame update
    void Start()
    {
        if (OnSceneLoaded != null)
        {
            OnSceneLoaded();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PhotonNetwork.IsMasterClient)
            GameManager.gameManager.PlayerFinished(other.gameObject);
    }
}
