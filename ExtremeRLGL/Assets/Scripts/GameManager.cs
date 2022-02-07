using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage 
{
    Waiting,
    Playing,
    Ending
}

public class GameManager : MonoBehaviour
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
    
    private List<FinishedPlayer> finishedPlayer;

    private void Awake()
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
    }


    // Update is called once per frame
    void Update()
    {
        // Counting down timer
        if (gameStage == GameStage.Playing)
        {
            timeLeft -= Time.deltaTime;
            if (timeLeft <= 0)
                GameEnd();
        }
    }

    public void GameStart()
    {
        StartCoroutine(StartingGame());
    }

    public void resetTimer()
    {
        timeLeft = gameLength;
    }

    IEnumerator StartingGame()
    {
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
        CreateBots();
        gameStage = GameStage.Playing;
    }

    public void CreateBots()
    {
        // hardcode 1 for now
        int numOfBot = 1;
        GameObject.Instantiate(botPrefab);
    }

    public void PlayerFinished(GameObject player)
    {
        finishedPlayer.Add(new FinishedPlayer(player, timeLeft, 1));
        LightManager.lightManager.TurnOffLights();
    }

    public void GameEnd() 
    {
        gameStage = GameStage.Ending;
    }
}
