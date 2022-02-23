using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RestartButton : MonoBehaviour
{
    private GameObject button;

    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.transform.GetChild(0).gameObject;
        button.SetActive(false);
        button.GetComponent<Button>().onClick.AddListener(RestartGame);
    }

    private void RestartGame()
    {
        Debug.Log("Restarting the game!");
        GameManager.gameManager.GameRestart();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.gameStage == GameStage.Ending && PhotonNetwork.IsMasterClient)
        {
            button.SetActive(true);
        }
        else 
        {
            button.SetActive(false);
        }
    }
}
