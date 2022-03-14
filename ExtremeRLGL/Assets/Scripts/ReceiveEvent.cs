using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class ReceiveEvent : MonoBehaviour, IOnEventCallback
{
    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == NetworkManager.HideUIEventCode)
        {
            object[] data = (object[])photonEvent.CustomData;
            string objectName = (string)data[0];
            GameObject ui = GameObject.Find(objectName);
            if (ui != null)
            {
                ui.SetActive(false);
            }
        } else if (eventCode == LightManager.PlayAudioSourceEventCode)
        {
            Debug.Log("Received event to play audio source.");
            object[] data = (object[])photonEvent.CustomData;
            string objectName = (string)data[0];
            AudioSource audioSource = GameObject.Find(objectName).GetComponent<AudioSource>();
            if (audioSource != null)
            {
                Debug.Log("Playing audio");
                audioSource.Play();
            }
        }
    }
}