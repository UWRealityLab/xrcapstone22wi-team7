using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class FrontUIManager : MonoBehaviour, IPunObservable
{
    public static FrontUIManager frontUIManager;

    [SerializeField]
    public TextMeshProUGUI warning;
    [SerializeField]
    public TextMeshProUGUI timeLeft;
    [SerializeField]
    public GameObject startButton;

    private PhotonView photonView;

    public void Awake()
    {
        frontUIManager = this;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(warning.gameObject.activeSelf);
            stream.SendNext(timeLeft.gameObject.activeSelf);
            stream.SendNext(warning.text);
            stream.SendNext(timeLeft.text);
        }
        else
        {
            // Network player, receive data
            warning.gameObject.SetActive((bool)stream.ReceiveNext());
            timeLeft.gameObject.SetActive((bool)stream.ReceiveNext());
            warning.text = (string)stream.ReceiveNext();
            timeLeft.text = (string)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);

        warning.gameObject.SetActive(false);
        timeLeft.gameObject.SetActive(false);

        if (!PhotonNetwork.IsMasterClient || !GameManager.isOnline)
        {
            startButton.SetActive(false);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
            timeLeft.text = (int)GameManager.timeLeft + "s";

    }

    public void OnButtonPressed()
    {
        GameManager.gameManager.GameStart();
    }

    public void ShowTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            timeLeft.gameObject.SetActive(true);
            Debug.Log("Showing timer");
        }
    }

    public void ShowWarning()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            warning.gameObject.SetActive(true);
            Debug.Log("Showing warning");
        }
    }

    public void SetWarning(string t)
    {
        if (PhotonNetwork.IsMasterClient)
            warning.text = t;
    }

    public void HideTimer()
    {
        if (PhotonNetwork.IsMasterClient)
            timeLeft.gameObject.SetActive(false);
    }

    public void HideWarning()
    {
        if (PhotonNetwork.IsMasterClient)
            warning.gameObject.SetActive(false);
    }

    
}
