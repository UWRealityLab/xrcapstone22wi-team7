using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtonReactor : MonoBehaviour
{
    public MenuButtonWatcher watcher;
    public bool alreadyOpen = false;

    public static GameObject miniMenu;

    void Start()
    {
        watcher.menuButtonPress.AddListener(onMenuButtonEvent);
        if (miniMenu == null)
        {
            miniMenu = GameObject.Find("Mini Menu");
        }
        miniMenu.SetActive(false);
    }

    public void onMenuButtonEvent(bool pressed)
    {
        bool validGameStage = GameManager.gameStage == GameStage.Playing || GameManager.gameStage == GameStage.Countdown;
        if (SceneManager.GetActiveScene().name == "MultiplayerGameScene" && validGameStage)
        {
            if (alreadyOpen && pressed)
            {
                closedMenu();
            }
            else if (!alreadyOpen && pressed)
            {
                miniMenu.SetActive(true);
                alreadyOpen = true;
            }
        }
    }

    public void closedMenu()
    {
        miniMenu.SetActive(false);
        alreadyOpen = false;
    }
}