using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rock : MonoBehaviour, IPunObservable
{
    public bool grabbed = false;
    public bool meGrabbed = false;
    private PhotonView photonView;

    // Start is called before the first frame update
    void Start()
    {
        photonView = PhotonView.Get(this);
        grabbed = false;
        meGrabbed = false;
    }

    // Update is called once per frame
    void Update()
    {
        
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
            if (other.CompareTag("Player") && other.GetComponent<PhotonView>().IsMine && !meGrabbed)
            {
                other.GetComponent<PlayerInteraction>().pushPlayer();
                PhotonNetwork.Destroy(gameObject);
                Debug.Log("Rock collided!");
            }
            else if (other.layer == LayerMask.NameToLayer("Self"))
            {
                Physics.IgnoreCollision(collision.collider, GetComponent<Collider>());
            }
        }   
    }   
}
