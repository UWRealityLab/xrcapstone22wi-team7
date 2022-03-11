using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject playerBody;
    public TextMeshProUGUI debugText;
    public float timeToHold = 3f;
    public float timeToFall = 4f;
    public bool stopped;
    public bool isRobot;

    public Animator animator;

    private PhotonView photonView;
    private XROrigin rig;
    private Camera mainCamera;
    private Camera fallenCamera;
    private float startTime;

    private PlayerPowerup playerPowerup;

    // Start is called before the first frame update
    void Start()
    {
        // getting interaction manager
        XRSimpleInteractable simple = FindObjectOfType<XRSimpleInteractable>();
        simple.interactionManager = GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>();

        photonView = PhotonView.Get(this);
        if (photonView.IsMine)
        {
            // Only show debug text for your player
            debugText.gameObject.SetActive(true);

            // don't interact with your own player
            simple.interactionLayers = InteractionLayerMask.GetMask("Self");
        } else
        {
            simple.interactionLayers = InteractionLayerMask.GetMask("Pushing");
        }

        // Set up cameras
        rig = FindObjectOfType<XROrigin>();
        mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        fallenCamera = GameObject.FindWithTag("FallCamera").GetComponent<Camera>();
        mainCamera.enabled = true;
        fallenCamera.enabled = false;
        stopped = false;

        playerPowerup = GetComponent<PlayerPowerup>();
    }

    // Update is called once per frame
    void Update()
    {
        // getting interaction manager again in case of scene change
        // maybe clean up code/make it so it doesn't always have to do this
        XRSimpleInteractable simple = FindObjectOfType<XRSimpleInteractable>();
        simple.interactionManager = GameObject.Find("XR Interaction Manager").GetComponent<XRInteractionManager>();
        if (photonView.IsMine)
        {
            // don't interact with your own player
            simple.interactionLayers = InteractionLayerMask.GetMask("Self");
        }
        else
        {
            simple.interactionLayers = InteractionLayerMask.GetMask("Pushing");
        }
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
        if (diff >= timeToHold && !playerPowerup.isActivate(PowerUpType.INVINCIBLE))
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
            // current fix to setting up camera on scene change is to just keep finding them, so when scene changes, it will find them again
            // if I make them DontDestroyOnLoad, the network model moves, but the player themselves don't see the movement
            if (!isRobot)
            {
                rig = FindObjectOfType<XROrigin>();
                mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                fallenCamera = GameObject.FindWithTag("FallCamera").GetComponent<Camera>();

                mainCamera.enabled = true;
                fallenCamera.enabled = false;

                // Move camera to where player is 
                Vector3 newCamPos = gameObject.transform.position;
                newCamPos.y = 0.5f;
                fallenCamera.transform.position = newCamPos;

                // Enable fallen camera (and disable rig to not see controller rays)
                fallenCamera.enabled = true;
                mainCamera.enabled = false;
                rig.gameObject.SetActive(false);
            }

            // Stop body from moving (stop tracking position) and rotate it so it "falls"
            animator.SetBool("isMoving", false);
            stopped = true;
            playerBody.transform.Rotate(83, 0, 0, Space.Self);

            rig.GetComponent<ClimbingMovement>().Reset();

            yield return new WaitForSeconds(seconds);

            if (!isRobot)
            {
                // Re-enable main camera
                rig.gameObject.SetActive(true);
                fallenCamera.enabled = false;
                mainCamera.enabled = true;
            }
            // Undo rotation and re-track movement
            playerBody.transform.Rotate(-83, 0, 0, Space.Self);
            stopped = false;
            
        }
    }
}
