using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    public delegate void TimeUpAction(string id);
    public static event TimeUpAction OnTimeUp;

    private static Dictionary<string, Timer> timers;
    private static List<string> removingTimers;

 

    // Start is called before the first frame update
    void Start()
    {
        timers = new Dictionary<string, Timer>();
        removingTimers = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (KeyValuePair<string, Timer> entry in timers)
        {
            Timer timer = entry.Value;
            if (timer.on) 
            {
                timer.timeLeft -= Time.deltaTime;
                if (timer.timeLeft <= 0)
                {
                    removingTimers.Add(entry.Key);
                    timer.on = false;
                    if (OnTimeUp != null)
                        OnTimeUp(entry.Key);
                }
            }
        }
  

        foreach (string timerId in removingTimers)
        {
            timers.Remove(timerId);
        }
        removingTimers.Clear();
    }

    public static void AddTimer(string id, float time)
    {

        timers.Add(id, new Timer(time));
        BeginTimer(id);
    }

    public static void BeginTimer(string id)
    {
        timers[id].on = true;
    }

    public static void PauseTimer(string id)
    {
        timers[id].on = false;
    }

    public static void ResetTimer(string id, float time)
    {
        timers[id].timeLeft = time;
    }

    public static float ReadTimer(string id)
    {
        return timers[id].timeLeft;
    }

    public static bool IsTimerOn(string id)
    {
        return timers[id].on;
    }

    public static void ClearTimers()
    {
        if (timers != null)
            timers.Clear();
    }


    class Timer
    {
        public float timeLeft;
        public bool on;

        public Timer(float timeLeft)
        {
            this.timeLeft = timeLeft;
            this.on = false;
        }
    }
}