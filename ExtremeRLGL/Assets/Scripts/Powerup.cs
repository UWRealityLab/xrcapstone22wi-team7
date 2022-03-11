using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;



public class Powerup : MonoBehaviour
{
    public PowerUpType type;
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
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerPowerup>().StartPowerUp(type);
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
