using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Haptics : MonoBehaviour
{
    public float secondsToWait = 3.2f;
    private bool selected = false;
    private bool sentHaptics = false;
    private XRBaseController controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = gameObject.GetComponent<XRBaseController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void startSelect()
    {
        selected = true;
        sentHaptics = false;
        if (controller != null)
        {
            controller.SendHapticImpulse(0.1f, 0.1f);
        }
        StartCoroutine(sendHapticsAfterSeconds(secondsToWait));
    }

    public void endSelect()
    {
        selected = false;
    }

    public IEnumerator sendHapticsAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (controller != null && selected && !sentHaptics)
        {
            controller.SendHapticImpulse(0.7f, 0.2f);
            sentHaptics = true;
        }
    }
}