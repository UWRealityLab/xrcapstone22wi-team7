using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.CoreUtils;

public class CanvasStartPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        XROrigin rig = FindObjectOfType<XROrigin>();
        gameObject.transform.position = rig.transform.position + new Vector3(0f, 1.4f, 2.4f);
    }

    // Update is called once per frame
    void Update()
    {
        XROrigin rig = FindObjectOfType<XROrigin>();
        gameObject.transform.position = rig.transform.position + new Vector3(0f, 1.4f, 2.4f);
    }
}
