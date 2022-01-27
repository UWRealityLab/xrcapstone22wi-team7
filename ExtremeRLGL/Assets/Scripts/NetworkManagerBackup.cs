 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManagerBackup : MonoBehaviourPunCallbacks
{
    [SerializeField] private byte maxPlayersPerRoom = 10;
    string gameVersion = "1";
    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        Debug.Log("Trying to connect to server...");
    }

    /*
     * Note: when overriding, most IDEs will by default implement a base call and fill that up for you automatically. 
     * As a general rule for MonoBehaviourPunCallbacks, never call the base method unless you override OnEnable() or OnDisable(). 
     * Always call the base class methods if you override OnEnable() and OnDisable().
     */
    public override void OnConnectedToMaster()
    {
        // base.OnConnectedToMaster();
        Debug.Log("Connected to server.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedRoom()
    {
        // base.OnJoinedRoom();
        Debug.Log("Joined a room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // base.OnPlayerEnteredRoom(newPlayer);
        Debug.Log("A new player joined the room.");
    }
}
