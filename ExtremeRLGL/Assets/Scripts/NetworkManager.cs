using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject menu;
    public GameObject connectingText;

    public GameObject roomUI;
    public GameObject createUI;
    public GameObject joinUI;
    public GameObject joinErrorText;
    public GameObject joinInput;
    public TextMeshProUGUI createdRoomCode;

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
        DontDestroyOnLoad(this.gameObject);
    }

    public void ConnectToServer()
    {
        connectingText.SetActive(true);
        StartCoroutine(RemoveAfterSeconds(2, connectingText));
        Debug.Log("Trying to connect to server...");
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }

    public void Disconnect()
    {
        Debug.Log("Disconnecting.");
        PhotonNetwork.Disconnect();
    }
    public void QuickMatch()
    {
        // The first we try to do is to join a potential existing room. 
        // If there is, good, else, we'll be called back with OnJoinRandomFailed()
        //if (PhotonNetwork.IsMasterClient)
        //{
        PhotonNetwork.LoadLevel("MultiplayerGameScene");
        //}
        PhotonNetwork.JoinRandomRoom();


        // Once enough players are in scene, we can start game? 
        // Or some sort of manual start?
    }

    public void CreateRoom()
    {
        // Generate a random 4 letter room name
        string roomName = "";
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        for (int i = 0; i < 4; i ++)
        {
            roomName += chars[Random.Range(0, chars.Length)];
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = maxPlayersPerRoom;
        roomOptions.IsVisible = false;  // not possible to join randomly
        roomOptions.IsOpen = true;  // let others join

        // Set text
        createdRoomCode.text = "<b>Room Code:</b> " + roomName;

        // Create and join room
        Debug.Log("Creating and joining room " + roomName);
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
        PhotonNetwork.LoadLevel("MultiplayerGameScene");
    }

    public void JoinRoom()
    {
        string roomName = joinInput.GetComponent<TMP_InputField>().text;
        Debug.Log("Trying to join room " + roomName);
        PhotonNetwork.JoinRoom(roomName.ToUpper());
    }

    public void LeaveRoom()
    {
        Debug.Log("Leaving room.");
        PhotonNetwork.LeaveRoom();
    }

    public void StartGame()
    {
        Debug.Log("Starting game.");
        PhotonNetwork.CurrentRoom.IsOpen = false;
        // TODO: Let person who created room start game (probably switch scene)
        //if (PhotonNetwork.IsMasterClient)
        //{
        //    PhotonNetwork.LoadLevel("MultiplayerGameScene");
        //}

    }
    IEnumerator RemoveAfterSeconds(int seconds, GameObject obj)
    {
        yield return new WaitForSeconds(seconds);
        obj.SetActive(false);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to master server.");
        PhotonNetwork.JoinLobby();

        // Hiding UI only if actually connected.
        // In case of no internet, player will not be able to succesfully connect (but they won't crash either).
        // Probably want to handle better later.
        connectingText.SetActive(false);
        menu.SetActive(false);
        roomUI.SetActive(true);
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby.");
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // We failed to join a random room, maybe none exists or they are all full - create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        // Create another room if failed (e.g. current name already used)
        Debug.Log("Failed to create room.");
        CreateRoom();
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room.");
        joinErrorText.SetActive(false);
        joinUI.SetActive(false);
        PhotonNetwork.LoadLevel("MultiplayerGameScene");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room: " + message);
        joinErrorText.SetActive(true);
        StartCoroutine(RemoveAfterSeconds(3, joinErrorText));
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined the room.");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
    }

}