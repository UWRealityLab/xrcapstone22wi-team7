using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LightManager : MonoBehaviour
{
    public double greenLightTimeMean;
    public double greenLightTimeStd;
    public double redLightTimeMean;
    public double redLightTimeStd;
    public PlayerMotion playerMotion;

    public Image [] lights;

    public static bool on;
    public static bool redLightOn;

    // Start is called before the first frame update
    void Start()
    {
        TurnOffLights();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        TimerManager.OnTimeUp += OnTimeUp;
    }

    private void OnDisable()
    {
        TimerManager.OnTimeUp -= OnTimeUp;
    }

    public void TurnOnLights()
    {
        TurnOffLights();
        redLightOn = false;
        on = true;
        TimerManager.AddTimer("redLightOnIn3", GetRandomTime(redLightTimeMean, redLightTimeStd));
        foreach (Image light in lights)
        {
            light.color = Color.green;
        }
    }

    public void TurnOffLights()
    {
        redLightOn = false;
        on = false;
        TimerManager.ClearTimers();
        foreach (Image light in lights)
        {
            light.color = Color.grey;
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

    private void OnTimeUp(string id) 
    {
        switch (id)
        {
            case "redLightOnIn3":
                lights[0].color = Color.red;
                TimerManager.AddTimer("redLightOnIn2", 1.0f);
                break;
            case "redLightOnIn2":
                lights[1].color = Color.red;
                TimerManager.AddTimer("redLightOnIn1", 1.0f);
                break;
            case "redLightOnIn1":
                lights[2].color = Color.red;
                TimerManager.AddTimer("redLightOnIn0", 1.0f);
                break;
            case "redLightOnIn0":
                lights[3].color = Color.red;
                redLightOn = true;
                playerMotion.ResetInitialPositions();
                TimerManager.AddTimer("greenLightOnIn0", GetRandomTime(greenLightTimeMean, greenLightTimeStd));
                break;
            case "greenLightOnIn0":
                TurnOnLights();
                break;
            default:
                Debug.Log("Didn't launch such timer!!!");
                break;
        }
    }
}
