using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class ClimbingInteractable : XRBaseInteractable
{
    // Variables
    public List<XRBaseInteractor> interactorList = new List<XRBaseInteractor>();
    public List<GameObject> gameObjectList = new List<GameObject>();
    private List<FixedJoint> fixedJointList = new List<FixedJoint>(); 
    private Rigidbody rigidBody = null;
    public LayerMask defaultLayer = 0;
    private bool isCleared = false;

    // Start is called before the first frame update
    void Start(){
        rigidBody = GetComponent<Rigidbody>();
    }

    // OnSelectEntering is called right before the interactor first initiates selection of an interactable
    protected override void OnSelectEntering(SelectEnterEventArgs args)
    {
        interactorList.Add(args.interactor);
        Debug.Log(args.interactor.name);

        // Executes if fixedJointList is empty
        if (fixedJointList.Count == 0) 
        {
            defaultLayer = gameObject.layer;
        }
        this.gameObject.SetLayerRecursively(LayerMask.NameToLayer("GrabbedObject"));

        // Sets layers for each game object in gameObjectList and each of their children
        foreach (GameObject gameObject in gameObjectList) 
        {
            gameObject.SetLayerRecursively(LayerMask.NameToLayer("GrabbedObject"));
        }
        args.interactor.gameObject.GetComponent<HandPhysicalMovement>().m_TargetToMove.gameObject.SetLayerRecursively(LayerMask.NameToLayer("GrabbingHand"));

        // Initialize variables used to get rays
        var attatchPos = args.interactor.GetComponent<HandPhysicalMovement>().m_TargetAttachPointTransform;
        bool isLeftHand = args.interactor.GetComponent<HandPhysicalMovement>().isLeftHand;
        var interval = 0.0175f;

        // Initialize variables used to change the position of the interactor
        float distBuff = 1000;
        Vector3 diff = Vector3.zero;
        for (int i = 1; i < 10; i++) {
            // Get rays using the attachPos, interval, and isLeftHand variables
            Ray ray1 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * interval * i + attatchPos.forward * interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));                
            Ray ray2 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * interval * i + attatchPos.forward * -interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray3 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * -interval * i + attatchPos.forward * interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray4 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * -interval * i + attatchPos.forward * -interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray5 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * 0f + attatchPos.forward * -interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray6 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * -interval * i + attatchPos.forward * 0f + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray7 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * 0f + attatchPos.forward * interval * i + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            Ray ray8 = headingRay(attatchPos.position, attatchPos.position + attatchPos.up * interval * i + attatchPos.forward * 0f + attatchPos.right * 0.1f * (isLeftHand == true ? 1 : -1));
            RaycastHit hit1, hit2, hit3, hit4, hit5, hit6, hit7, hit8;

            // Executes if the sphere sweep intersects any collider
            if (Physics.SphereCast(ray1, 0.001f, out hit1, 1, ~(1 << 20))) {
                if (hit1.rigidbody) {
                    if (hit1.rigidbody == rigidBody) {
                        if (hit1.distance <= distBuff) {
                            distBuff = hit1.distance;
                            diff = hit1.point;
                        }
                    }
                }
            }           
            if (Physics.SphereCast(ray2, 0.001f, out hit2, 1, ~(1 << 20))) {
                if (hit2.rigidbody) {
                    if (hit2.rigidbody == rigidBody) {
                        if (hit2.distance <= distBuff) {
                            distBuff = hit2.distance;
                            diff = hit2.point;
                        }
                    }
                }    
            }   
            if (Physics.SphereCast(ray3, 0.001f, out hit3, 1, ~(1 << 20))) {
                if (hit3.rigidbody) {
                    if( hit3.rigidbody == rigidBody) {
                        if (hit3.distance <= distBuff) {
                            distBuff = hit3.distance;
                            diff = hit3.point;
                        }    
                    }
                }                
            }   
            if (Physics.SphereCast(ray4, 0.001f, out hit4, 1, ~(1 << 20))) {
                if (hit4.rigidbody) {
                    if (hit4.rigidbody == rigidBody) {
                        if (hit4.distance <= distBuff) {
                            distBuff = hit4.distance;
                            diff = hit4.point;
                        }        
                    }
                }                                       
            }   
            if (Physics.SphereCast(ray5, 0.001f, out hit5, 1, ~(1 << 20))) {
                if (hit5.rigidbody) {
                    if (hit5.rigidbody == rigidBody) {
                        if (hit5.distance <= distBuff) {
                            distBuff = hit5.distance;
                            diff = hit5.point;
                        }                
                    }
                }                                
            }   
            if (Physics.SphereCast(ray6, 0.001f, out hit6, 1, ~(1 << 20))) {
                if (hit6.rigidbody) {
                    if (hit6.rigidbody == rigidBody) {
                        if (hit6.distance <= distBuff) {
                            distBuff = hit6.distance;
                            diff = hit6.point;
                        }        
                    }
                }            
            }   
            if (Physics.SphereCast(ray7, 0.001f, out hit7, 1, ~(1 << 20))) {
                if (hit7.rigidbody) {
                    if (hit7.rigidbody == rigidBody) {
                        if (hit7.distance <= distBuff) {
                            distBuff = hit7.distance;
                            diff = hit7.point;
                        }                
                    }
                }    
            }   
            if (Physics.SphereCast(ray8, 0.001f, out hit8, 1, ~(1 << 20))) {
                if (hit8.rigidbody) {
                    if (hit8.rigidbody == rigidBody) {
                        if (hit8.distance <= distBuff) {
                            distBuff = hit8.distance;
                            diff = hit8.point;
                        }            
                    }
                }    
            } 
        }

        // Executes if distBuff and diff meet certain thresholds
        if (distBuff != 1000 && distBuff >= 0.02f && diff != Vector3.zero) 
        {
            args.interactor.GetComponent<HandPhysicalMovement>().rigidBody.transform.position += diff - attatchPos.position;
        }
        FixedJoint joint = args.interactor.GetComponent<HandPhysicalMovement>().rigidBody.gameObject.AddComponent<FixedJoint>();
        joint.connectedBody = rigidBody;
        fixedJointList.Add(joint);
        rigidBody.interpolation = RigidbodyInterpolation.Interpolate;
        isCleared = false;
    }

    // OnSelectExiting is called right before the interactor ends selection of an interactable
    protected override void OnSelectExiting(SelectExitEventArgs args)
    {
        // Remove interactor and fixed joint from interactorList and fixedJointList
        interactorList.Remove(args.interactor);
        fixedJointList.Remove(args.interactor.GetComponent<HandPhysicalMovement>().rigidBody.gameObject.GetComponent<FixedJoint>());

        // Executes if interactorList is empty and isCleared is false
        if (interactorList.Count == 0 && !isCleared)
        {
            this.gameObject.SetLayerRecursively(defaultLayer);

            // Sets layers for each game object in gameObjectList and each of their children
            foreach (GameObject optionGameObject in gameObjectList)
            {
                optionGameObject.SetLayerRecursively(defaultLayer);
            }
        }
        args.interactor.gameObject.GetComponent<HandPhysicalMovement>().m_TargetToMove.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Hand"));
        GameObject.Destroy(args.interactor.GetComponent<HandPhysicalMovement>().rigidBody.gameObject.GetComponent<FixedJoint>());
    }

    // Release is called when the user is not grabbing the interactable
    public void Release()
    {
        foreach (XRBaseInteractor interactor in interactorList) 
        {
            interactor.gameObject.GetComponent<HandPhysicalMovement>().m_TargetToMove.gameObject.SetLayerRecursively(LayerMask.NameToLayer("Hand"));
            GameObject.Destroy(interactor.GetComponent<HandPhysicalMovement>().rigidBody.gameObject.GetComponent<FixedJoint>());
        }

        // Clears interactorList & fixedJointList and sets isCleared to true
        interactorList.Clear();
        fixedJointList.Clear();
        isCleared = true;
    }

    // Creates a new ray using the start and end of a vector
    Ray headingRay(Vector3 start, Vector3 end) {
        var diff = end - start;
        return new Ray(start, diff / diff.magnitude);
    }
}

public static class GameObjectExtensions
{
    // Sets layers for all child objects, including this object
    public static void SetLayerRecursively(this GameObject self, int layer)
    {
        self.layer = layer;
        foreach (Transform n in self.transform)
        {
            SetLayerRecursively(n.gameObject, layer);
        }
    }
}