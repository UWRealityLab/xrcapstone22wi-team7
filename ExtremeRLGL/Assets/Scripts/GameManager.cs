using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum GameStage 
{
    Waiting,
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
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameStage = GameStage.Waiting;
        playerObjects = new Dictionary<int, GameObject>();
        isPausing = false;
        isOnline = false;
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

    public void GameStart()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            playerNum = players.Length;
            Debug.Log("Starting the game");
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
        startLine = GameObject.FindGameObjectWithTag("StartLine").GetComponent<Collider>();
        // Turning on the UI that goes along with the player
        FrontUIManager.frontUIManager.ShowWarning();
        for (int i = 10; i >= 0; i--)
        {
            FrontUIManager.frontUIManager.SetWarning("The game is starting in " + i + " seconds...");
            yield return new WaitForSeconds(1.0f);
        }
        FrontUIManager.frontUIManager.HideWarning();
        resetTimer();
        FrontUIManager.frontUIManager.ShowTimer();
        // CreateBots();
        gameStage = GameStage.Playing;
    }

    public void CreateBots()
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            // hardcode 1 for now
            int numOfBot = 1;
            Vector3 randomPoint = new Vector3(
               Random.Range(startLine.bounds.min.x, startLine.bounds.max.x),
               Random.Range(startLine.bounds.min.y, startLine.bounds.max.y),
               Random.Range(startLine.bounds.min.z, startLine.bounds.max.z)
            );
            for (int i = 0; i < numOfBot; i++)
                PhotonNetwork.Instantiate(botPrefab.name, randomPoint, transform.rotation);
        }
    }

    public void PlayerFinished(GameObject player)
    {
        if (!isOnline || PhotonNetwork.IsMasterClient)
        {
            finishedPlayer.Add(new FinishedPlayer(player, timeLeft, 1));
            player.GetComponent<NetworkPlayer>().stopped = true;
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

    
}
