using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotion : MonoBehaviour
{
    public float HeadsetPositionThreshold;
    public float HeadsetRotationThreshold;
    public float ControllerPositionThreshold;
    public float ControllerRotationThreshold;

    private GameObject LeftController;
    private GameObject RightController;
    private GameObject Headset;

    public bool moved;

    private Vector3 HOldPosition;
    private Vector3 HOldRotation;
    private Vector3 LOldPosition;
    private Vector3 LOldRotation;
    private Vector3 ROldPosition;
    private Vector3 ROldRotation;


    private void Awake()
    {
        LeftController = GameObject.Find("LeftHand Controller");
        RightController = GameObject.Find("RightHand Controller");
        Headset = GameObject.FindWithTag("MainCamera");
    }

    private void OnEnable()
    {
        TimerManager.OnTimeUp += Setback;
    }

    private void OnDisable()
    {
        TimerManager.OnTimeUp -= Setback;
    }

    // Start is called before the first frame update
    void Start()
    {
        HOldPosition = Headset.transform.position;
        HOldRotation = Headset.transform.rotation.eulerAngles;
        LOldPosition = LeftController.transform.position;
        LOldRotation = LeftController.transform.rotation.eulerAngles;
        ROldPosition = RightController.transform.position;
        ROldRotation = RightController.transform.rotation.eulerAngles;

    }

    // Update is called once per frame
    void Update()
    {
        Vector3 HNewPosition = Headset.transform.position;
        Vector3 HNewRotation = Headset.transform.rotation.eulerAngles;
        Vector3 LNewPosition = LeftController.transform.position;
        Vector3 LNewRotation = LeftController.transform.rotation.eulerAngles;
        Vector3 RNewPosition = RightController.transform.position;
        Vector3 RNewRotation = RightController.transform.rotation.eulerAngles;

        if (!moved)
        {
            if ((HNewPosition - HOldPosition).magnitude * 10000 > HeadsetPositionThreshold ||
                (HNewRotation - HOldRotation).magnitude * 10 > HeadsetRotationThreshold ||
                (LNewPosition - LOldPosition).magnitude * 10000 > ControllerPositionThreshold ||
                (LNewRotation - LOldRotation).magnitude * 10 > ControllerRotationThreshold ||
                (RNewPosition - ROldPosition).magnitude * 10000 > ControllerPositionThreshold ||
                (RNewRotation - ROldRotation).magnitude * 10 > ControllerRotationThreshold)
            {
                Debug.Log((HNewPosition - HOldPosition).magnitude * 10000 - HeadsetPositionThreshold);
                Debug.Log((HNewRotation - HOldRotation).magnitude * 10 - HeadsetRotationThreshold);
                Debug.Log((LNewPosition - LOldPosition).magnitude * 10000 - ControllerPositionThreshold);
                Debug.Log((LNewRotation - LOldRotation).magnitude * 10 - ControllerRotationThreshold);
                Debug.Log((RNewPosition - ROldPosition).magnitude * 10000 - ControllerPositionThreshold);
                Debug.Log((RNewRotation - ROldRotation).magnitude * 10 - ControllerRotationThreshold);

                moved = true;
                Debug.Log("Moved at " + Time.time);
                TimerManager.AddTimer("Reevulate moving", 2);
            }
        }

        HOldPosition = HNewPosition;
        HOldRotation = HNewRotation;
        LOldPosition = LNewPosition;
        LOldRotation = LNewRotation;
        ROldPosition = RNewPosition;
        ROldRotation = RNewRotation;
    }

    void Setback(string id)
    {

        if (string.Equals(id, "Reevulate moving"))
        {
            moved = false;
            Debug.Log("Cleared at " + Time.time);
        }
    }

}
