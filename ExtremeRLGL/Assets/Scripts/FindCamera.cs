using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class FindCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonView photonView = PhotonView.Get(this);
        if (!photonView.IsMine)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.GetComponent<Canvas>().worldCamera == null)
        {
            gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log("try to find camera"); 
        }
           
    }

    public void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene Loaded");
        Debug.Log(scene.name);
        if (scene.name == "MultiplayerGameScene")
        {
            gameObject.GetComponent<Canvas>().worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            Debug.Log("Found Camera:");
            Debug.Log(GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>());
        }
    }
}
