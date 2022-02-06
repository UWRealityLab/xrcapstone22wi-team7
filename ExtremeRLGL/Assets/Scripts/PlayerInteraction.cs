using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.XR.CoreUtils;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject playerBody;
    public TextMeshProUGUI debugText;
    public float timeToHold = 3f;
    public float timeToFall = 4f;

    private PhotonView photonView;
    private XROrigin rig;
    private Camera mainCamera;
    private Camera fallenCamera;
    private NetworkPlayer networkPlayer;
    private float startTime;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        if (photonView.IsMine)
        {
            // Only show debug text for your player
            debugText.gameObject.SetActive(true);
        }

        // Set up cameras
        rig = FindObjectOfType<XROrigin>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        fallenCamera = GameObject.FindWithTag("FallCamera").GetComponent<Camera>();
        mainCamera.enabled = true;
        fallenCamera.enabled = false;

        networkPlayer = GetComponent<NetworkPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void startNetworkedPush()
    {
        startedHolding();
        photonView.RPC("updateDebugText", RpcTarget.All, "Someone is about to push you!");
    }

    public void endNetworkedPush()
    {
        endedHolding();
        photonView.RPC("updateDebugText", RpcTarget.All, "");
    }

    public void startedHolding()
    {
        startTime = Time.time;
    }

    public void endedHolding()
    {
        float endTime = Time.time;
        float diff = endTime - startTime;
        if (diff >= timeToHold)
        {
            // actually push if charged long enough
            photonView.RPC("pushPlayer", RpcTarget.All);
        }
    }

    [PunRPC]
    public void updateDebugText(string newText)
    {
        if (photonView.IsMine)
        {
            debugText.text = newText;
        }
    }

    [PunRPC]
    public void pushPlayer()
    {
        if (photonView.IsMine)
        {
            StartCoroutine(fallForDuration(timeToFall));
        }
    }

    public IEnumerator fallForDuration(float seconds)
    {
        if (photonView.IsMine)
        {
            // Stop body from moving (stop tracking position) and rotate it so it "falls"
            networkPlayer.stopped = true;
            playerBody.transform.Rotate(90, 0, 0, Space.Self);

            // Move camera to where player is 
            fallenCamera.transform.position = gameObject.transform.position;

            // Enable fallen camera (and disable rig to not see controller rays)
            fallenCamera.enabled = true;
            mainCamera.enabled = false;
            rig.gameObject.SetActive(false);

            yield return new WaitForSeconds(seconds);
            
            // Re-enable main camera
            rig.gameObject.SetActive(true);
            fallenCamera.enabled = false;
            mainCamera.enabled = true;

            // Undo rotation and re-track movement
            playerBody.transform.Rotate(-90, 0, 0, Space.Self);
            networkPlayer.stopped = false;
        }
    }
}
