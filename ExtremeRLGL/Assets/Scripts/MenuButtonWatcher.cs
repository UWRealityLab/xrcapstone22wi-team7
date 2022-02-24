using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

[System.Serializable]
public class MenuButtonEvent : UnityEvent<bool> { }

public class MenuButtonWatcher : MonoBehaviour
{
    public MenuButtonEvent menuButtonPress;

    private bool lastButtonState = false;
    private List<InputDevice> devicesWithMenuButton;

    private void Awake()
    {
        if (menuButtonPress == null)
        {
            menuButtonPress = new MenuButtonEvent();
        }

        devicesWithMenuButton = new List<InputDevice>();
    }

    void OnEnable()
    {
        List<InputDevice> allDevices = new List<InputDevice>();
        InputDevices.GetDevices(allDevices);
        foreach (InputDevice device in allDevices)
            InputDevices_deviceConnected(device);

        InputDevices.deviceConnected += InputDevices_deviceConnected;
        InputDevices.deviceDisconnected += InputDevices_deviceDisconnected;
    }

    private void OnDisable()
    {
        InputDevices.deviceConnected -= InputDevices_deviceConnected;
        InputDevices.deviceDisconnected -= InputDevices_deviceDisconnected;
        devicesWithMenuButton.Clear();
    }

    private void InputDevices_deviceConnected(InputDevice device)
    {
        bool discardedValue;
        if (device.TryGetFeatureValue(CommonUsages.menuButton, out discardedValue))
        {
            devicesWithMenuButton.Add(device);
        }
    }

    private void InputDevices_deviceDisconnected(InputDevice device)
    {
        if (devicesWithMenuButton.Contains(device))
            devicesWithMenuButton.Remove(device);
    }
    void Update()
    {
        bool tempState = false;
        foreach (var device in devicesWithMenuButton)
        {
            bool menuButtonState = false;
            tempState = device.TryGetFeatureValue(CommonUsages.menuButton, out menuButtonState) // did get a value
                        && menuButtonState // the value we got
                        || tempState; // cumulative result from other controllers
        }

        if (tempState != lastButtonState) // Button state changed since last frame
        {
            menuButtonPress.Invoke(tempState);
            lastButtonState = tempState;
        }
    }
}