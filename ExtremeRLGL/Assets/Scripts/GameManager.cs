using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameStage 
{
    Paused,
    Playing
}

public class GameManager : MonoBehaviour
{
    private static Dictionary<int, GameObject> playerObjects;
    public static bool isPausing;

    // Start is called before the first frame update
    void Start()
    {
        playerObjects = new Dictionary<int, GameObject>();
        isPausing = false;
    }

    private void OnEnable()
    {
        TimerManager.OnTimeUp += this.OnTimerUp;
    }

    private void OnDisable()
    {
        TimerManager.OnTimeUp -= this.OnTimerUp;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public static void RefreshPlayerObjects()
    {
        playerObjects.Clear();
        GameObject[] currentPlayerObjects = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject playerObject in currentPlayerObjects)
        {
            playerObjects.Add(playerObject.GetComponent<PlayerState>().ID, playerObject);
            playerObject.GetComponent<PlayerState>().joined = true;
        }
    }

    public static void Pause(float time)
    {
        if (time > 0)
            TimerManager.AddTimer("Game pause", time);
        isPausing = true;
    }

    public static void Resume()
    {
        isPausing = false;
    }

    private void OnTimerUp(string id)
    {
        if (id == "Game pause")
        {
            isPausing = false;
        }
    }
}
