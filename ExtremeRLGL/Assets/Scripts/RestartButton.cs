using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RestartButton : MonoBehaviour
{
    private GameObject restartButton;
    private GameObject leaveButton;

    // Start is called before the first frame update
    void Start()
    {
        restartButton = gameObject.transform.GetChild(0).gameObject;
        restartButton.SetActive(false);

        leaveButton = gameObject.transform.GetChild(1).gameObject;
        leaveButton.SetActive(false);
    }

    public void RestartGame()
    {
        Debug.Log("Restarting the game!");
        GameManager.gameManager.GameRestart();
    }

    public void LeaveGame()
    {
        PhotonNetwork.Disconnect();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameStage == GameStage.Ending && PhotonNetwork.IsMasterClient)
        {
            restartButton.SetActive(true);
            // leaveButton.SetActive(true);
        }
        else 
        {
            restartButton.SetActive(false);
            leaveButton.SetActive(false);
        }
    }
}
