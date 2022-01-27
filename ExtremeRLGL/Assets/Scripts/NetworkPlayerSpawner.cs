using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        // base.OnJoinedRoom();
        Debug.Log("Spawning player.");
        spawnedPlayerPrefab = PhotonNetwork.Instantiate(playerPrefab.name, transform.position, transform.rotation, 0);
    }

    public override void OnLeftRoom()
    {
        // base.OnLeftRoom();
        Debug.Log("Destroying player.");
        PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
