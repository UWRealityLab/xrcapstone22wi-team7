using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Unity.XR.CoreUtils;
public class FlashBomb : MonoBehaviour, IPunObservable
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

    public void Grabbed()
    {
        Debug.Log("Got Grabbed");
        grabbed = true;
        meGrabbed = true;
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
            if (other.layer == LayerMask.NameToLayer("Self"))
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
            else if (canBeSeen)
            {
                XROrigin rig = FindObjectOfType<XROrigin>();
                rig.GetComponent<Flash>().StartFlash(Color.white, 0.5f);
                PhotonNetwork.Destroy(gameObject);
                Debug.Log("Flash bomb collided!");
            }
        }

    }
}
