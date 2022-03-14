using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class LightManager : MonoBehaviour, IPunObservable
{
    private PhotonView photonView;

    public static LightManager lightManager;
    public double greenLightTimeMean;
    public double greenLightTimeStd;
    public double redLightTimeMean;
    public double redLightTimeStd;

    public Image[] lights;
    public GameObject lightPlate;

    private GameStage previousGameStage;
    private static bool on;
    private static bool[] redLightOn;

    private IEnumerator turningThread;

    public AudioSource audioSource;
    public AudioClip beepSound;

    public const byte PlayAudioSourceEventCode = 2;


    public void Awake()
    {
        lightManager = this;
    }

    public static bool RedlightAllOn() 
    {
        if (!on) return false;

        foreach (bool b in redLightOn)
        {
            if (!b) return false;
        }
        return true;
    }

    // Start is called before the first frame update
    void Start()
    {
        on = false;
        redLightOn = new bool[4];
        previousGameStage = GameStage.Waiting;
        turningThread = TurnOnLightThread();
        photonView = PhotonView.Get(this);
        TurnOffLights();

    }

    // Update is called once per frame
    void Update()
    {
        if (previousGameStage != GameManager.gameStage && PhotonNetwork.IsMasterClient)
        {
            if (GameManager.gameStage == GameStage.Playing)
            {
                Debug.Log("Turn on lights!");
                TurnOnLights();
            }
            else if (GameManager.gameStage == GameStage.Ending)
            {
                Debug.Log("Turn off lights!");
                TurnOffLights();
            }
            previousGameStage = GameManager.gameStage;
        }

        for (int i = 0; i < redLightOn.Length; i++)
        {
            if (!on)
                SetActive(false);
            else
                SetActive(true);

            if (redLightOn[i])
                lights[i].color = Color.red;
            else
                lights[i].color = Color.green;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(on);
            stream.SendNext(redLightOn);
        }
        else
        {
            // Network player, receive data
            on = (bool)stream.ReceiveNext();
            redLightOn = (bool[])stream.ReceiveNext();
        }
    }

    IEnumerator TurnOnLightThread()
    {
        while (on)
        {
            yield return new WaitForSeconds(GetRandomTime(greenLightTimeMean, greenLightTimeStd));
            redLightOn[0] = true;
            SendPlayAudioSourceEvent("Main Camera"); // main camera has audio source attached
            yield return new WaitForSeconds(1.0f);
            redLightOn[1] = true;
            SendPlayAudioSourceEvent("Main Camera");
            yield return new WaitForSeconds(1.0f);
            redLightOn[2] = true;
            SendPlayAudioSourceEvent("Main Camera");
            yield return new WaitForSeconds(1.0f);
            redLightOn[3] = true;
            SendPlayAudioSourceEvent("Main Camera");
            yield return new WaitForSeconds(GetRandomTime(redLightTimeMean, redLightTimeStd));
            
            for (int i = 0; i < redLightOn.Length; i++)
            {
                redLightOn[i] = false;
            }
            Debug.Log("Turning every light back to green");
        }
    }

    public void SendPlayAudioSourceEvent(string objectName)
    {
        object[] content = new object[] { objectName };
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.All };
        PhotonNetwork.RaiseEvent(PlayAudioSourceEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }


    public void TurnOnLights()
    {
        Debug.Log("try to turn on lights");
        if (!GameManager.isOnline || PhotonNetwork.IsMasterClient) 
        {
            Debug.Log("Turned on lights");
            TurnOffLights();
            for (int i = 0; i < redLightOn.Length; i++)
            {
                redLightOn[i] = false;
            }
            on = true;
            SetActive(true);
            StartCoroutine(turningThread);
        }
        
    }

    public void TurnOffLights()
    {
        if (!GameManager.isOnline || PhotonNetwork.IsMasterClient)
        {
            StopCoroutine(turningThread);
            on = false;
            for (int i = 0; i < redLightOn.Length; i++)
            {
                redLightOn[i] = false;
            }
        }
        
    }

    private float GetRandomTime(double mean, double stdDev)
    {
        System.Random rand = new System.Random(); //reuse this if you are generating many
        double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
        double u2 = 1.0 - rand.NextDouble();
        double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) *
                     System.Math.Sin(2.0 * System.Math.PI * u2); //random normal(0,1)
        double randNormal =
                     mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
        return (float)randNormal;
    }

    private void SetActive(bool active)
    {
        lightPlate.SetActive(active);
    }
}