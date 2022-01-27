 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject connectButton;
    public GameObject successText;
    public GameObject failText;

    [SerializeField] private byte maxPlayersPerRoom = 10;
    string gameVersion = "1";

    /* FROM: https://doc.photonengine.com/en-us/pun/current/demos-and-tutorials/pun-basics-tutorial/lobby
     * Note: when overriding, most IDEs will by default implement a base call and fill that up for you automatically. 
     * As a general rule for MonoBehaviourPunCallbacks, never call the base method unless you override OnEnable() or OnDisable(). 
     * Always call the base class methods if you override OnEnable() and OnDisable().
     */

    void Awake()
    {
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // ConnectToServer();
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
        Debug.Log("Trying to connect to server...");
    }

    // currently not using - hardcoded room 1
    public void InitializeRoom()
    {
        Debug.Log("Initializing room.");
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;

        PhotonNetwork.JoinOrCreateRoom("Room 1", roomOptions, TypedLobby.Default);
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to master server.");
        connectButton.SetActive(false);
        successText.SetActive(true);

        StartCoroutine(RemoveAfterSeconds(3, successText));

        // PhotonNetwork.JoinLobby();

        // The first we try to do is to join a potential existing room. 
        // If there is, good, else, we'll be called back with OnJoinRandomFailed()
        PhotonNetwork.JoinRandomRoom();
    }
    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // We failed to join a random room, maybe none exists or they are all full - create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room.");
    }
}
