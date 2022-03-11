using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class WaterMultiplayer : MonoBehaviour
{
    private PhotonView photonView;

    // Running and rowing movement scripts
    private RunningMovementMultiplayer runningMovement;
    private RowingMovementMultiplayer rowingMovement;

    public GameObject boat;
    public Collider boatCollider;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            // Get components for running and rowing movement scripts
            runningMovement = GetComponent<RunningMovementMultiplayer>();
            rowingMovement = GetComponent<RowingMovementMultiplayer>();
            boatCollider = boat.GetComponent<MeshCollider>();
        }
    }

    // OnTriggerEnter is called when a GameObject collides with another GameObject.
    private void OnTriggerEnter(Collider other)
    {
        if (photonView.IsMine && GameManager.gameStage == GameStage.Playing)
        {
            // Executes if this collider touches an object with the "water" tag
            if (other.gameObject.CompareTag("RowingStart"))
            {
                // GameObject.FindGameObjectWithTag("Water").GetComponent<Renderer>().material.color = Color.green;
                runningMovement.enabled = false;
                rowingMovement.enabled = true;
                photonView.RPC("showBoat", RpcTarget.All);
                boatCollider.enabled = true;
            }

            // Executes if this collider touches an object with the "water" tag
            if (other.gameObject.CompareTag("RowingEnd"))
            {
                // GameObject.FindGameObjectWithTag("Water").GetComponent<Renderer>().material.color = Color.green;
                runningMovement.enabled = true;
                rowingMovement.enabled = false;
                photonView.RPC("hideBoat", RpcTarget.All);
                boatCollider.enabled = false;
            }
        }
    }


    //// OnTriggerExit is called when the Collider has stopped touching the trigger.
    //private void OnTriggerExit(Collider other)
    //{
    //    if (photonView.IsMine && GameManager.gameStage == GameStage.Playing)
    //    {
    //        // Executes if this collider touches an object with the "water" tag
    //        if (other.gameObject.CompareTag("Water"))
    //        {
    //            // GameObject.FindGameObjectWithTag("Water").GetComponent<Renderer>().material.color = Color.red;
    //            runningMovement.enabled = true;
    //            rowingMovement.enabled = false;
    //            photonView.RPC("hideBoat", RpcTarget.All);
    //        }
    //    }
    //}

    [PunRPC]
    private void showBoat()
    {
        boat.SetActive(true);
    }

    [PunRPC]
    private void hideBoat()
    {
        boat.SetActive(false);
    }
}