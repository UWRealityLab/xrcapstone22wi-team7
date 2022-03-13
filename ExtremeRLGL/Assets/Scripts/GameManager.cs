using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum GameStage 
{
    Waiting,
    Countdown,
    Playing,
    Ending
}

public class GameManager : MonoBehaviour, IPunObservable
{
    

    struct FinishedPlayer
    {
        public GameObject player;
        public float time;
        public int rank;

        public FinishedPlayer(GameObject player, float time, int rank)
        {
            this.player = player;
            this.time = time;
            this.rank = rank;
        }
    }

    public static GameManager gameManager;
    private static Dictionary<int, GameObject> playerObjects;
    public static GameStage gameStage;
    public static bool isPausing;
    public static bool isOnline;
    public float gameLength;
    public static float timeLeft;
    public GameObject botPrefab;
    private Collider startLine;
    private int playerNum;
    private PhotonView photonView;

    private List<FinishedPlayer> finishedPlayer;


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(gameStage);
            stream.SendNext(timeLeft);
        }
        else
        {
            // Network player, receive data
            gameStage = (GameStage)stream.ReceiveNext();
            timeLeft = (float)stream.ReceiveNext();
        }
    }

    void Awake()
    {
        gameManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStage = GameStage.Waiting;
        playerObjects = new Dictionary<int, GameObject>();
        isPausing = false;
        isOnline = false;
        photonView = PhotonView.Get(this);    
    }


    // Update is called once per frame
    void Update()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            // Counting down timer
            if (gameStage == GameStage.Playing)
            {
                timeLeft -= Time.deltaTime;
                if (timeLeft <= 0)
                    GameEnd();
            }
        }
        
    }

    [PunRPC]
    public void ResetPlayerPosition()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            player.GetComponent<Teleport>().ToStartLine();
        }
        Debug.Log("Player position reset");
    }

    public void GameStart()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            playerNum = players.Length;
            Debug.Log("Starting the game");
            startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
            photonView.RPC("ResetPlayerPosition", RpcTarget.All);
            CreateBots();
            StartCoroutine(StartingGame());
        }
    }

    public void resetTimer()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
            timeLeft = gameLength;
    }

    IEnumerator StartingGame()
    {
        // Turning on the UI that goes along with the player
        gameStage = GameStage.Countdown;
        FrontUIManager.frontUIManager.ShowWarning();
        for (int i = 10; i >= 0; i--)
        {
            FrontUIManager.frontUIManager.SetWarning("The game is starting in " + i + " seconds...");
            yield return new WaitForSeconds(1.0f);
        }
        FrontUIManager.frontUIManager.HideWarning();
        resetTimer();
        FrontUIManager.frontUIManager.ShowTimer();
        gameStage = GameStage.Playing;
    }

    public void CreateBots()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            // hardcode 1 for now
            int numOfBot = 3;
            for (int i = 0; i < numOfBot; i++)
                PhotonNetwork.Instantiate(botPrefab.name, startLine.GetComponent<StartLine>().GetNextPos(), transform.rotation);
        }
    }

    public void PlayerFinished(GameObject player)
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            finishedPlayer.Add(new FinishedPlayer(player, timeLeft, 1));
            player.GetComponent<PlayerInteraction>().stopped = true;
        }
    }

    public void GameEnd() 
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            FrontUIManager.frontUIManager.ShowWarning();
            FrontUIManager.frontUIManager.SetWarning("Game Over");
            Debug.Log("Game Over");
            gameStage = GameStage.Ending;
        }
    }

    public void GameRestart()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject[] bots = GameObject.FindGameObjectsWithTag("Bot");
            foreach (GameObject bot in bots)
            {
                PhotonNetwork.Destroy(bot);
            }
            gameStage = GameStage.Waiting;
            FrontUIManager.frontUIManager.HideTimer();
            GameStart();
        }
    }

}
