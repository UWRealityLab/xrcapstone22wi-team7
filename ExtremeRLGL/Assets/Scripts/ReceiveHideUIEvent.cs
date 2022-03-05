using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun;

public class ReceiveHideUIEvent : MonoBehaviour, IOnEventCallback
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
        }
    }
}