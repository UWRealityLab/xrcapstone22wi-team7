using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
public class FlashBomb : MonoBehaviour
{
    public bool grabbed = false;
    public bool meGrabbed = false;
    public bool canBeSeen = false;
    private PhotonView photonView;
    private GameObject myPlayer;
    private Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        grabbed = false;
        meGrabbed = false;
        canBeSeen = false;

        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (player.GetComponent<PhotonView>().IsMine)
            {
                myPlayer = player;
            }
        }

        camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        GetComponent<Rigidbody>().isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (myPlayer == null)
        {
            foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponent<PhotonView>().IsMine)
                {
                    myPlayer = player;
                }
            }
        }

        
        // canBeSeen = GetComponent<Renderer>().isVisible;
        Vector3 screenPoint = camera.WorldToViewportPoint(transform.position);
        if (screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1)
        {
            canBeSeen = true;
        }
        else
        {
            canBeSeen = false;
        }
    }

    /*
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(grabbed);
        }
        else
        {
            // Network player, receive data
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            grabbed = (bool)stream.ReceiveNext();
        }
    }
    */

    public void Grabbed()
    {
        Debug.Log("Got Grabbed");
        photonView.RPC("AllGrabbed", RpcTarget.All);
        meGrabbed = true;
    }

    [PunRPC]
    public void AllGrabbed()
    {
        grabbed = true;
        GetComponent<Rigidbody>().isKinematic = false;
    }

    [PunRPC]
    public void DestroyAll()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        if (!grabbed)
        {
            Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
        }
        else
        {
            if (other.layer == LayerMask.NameToLayer("Self") && meGrabbed)
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
            else if (canBeSeen)
            {
                XROrigin rig = FindObjectOfType<XROrigin>();
                rig.GetComponent<Flash>().StartFlash(Color.white, 0.5f);
                photonView.RPC("DestroyAll", RpcTarget.All);
                Debug.Log("Flash bomb collided!");
            }
        }

    }
}
