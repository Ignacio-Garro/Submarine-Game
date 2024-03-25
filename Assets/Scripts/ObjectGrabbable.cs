using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectGrabbable : MonoBehaviour, IGrabbableObject
{
    private Rigidbody objectRigidbody;
   
    

    // Store previous position for velocity calculation
    private Vector3 previousPosition;
    [SerializeField]private Vector3 momentum;
    [SerializeField]private Vector3 finalMomentum;

    private Transform objectGrabPointTransfrom;
    private Collider targetCollider;


    private void Awake(){
        objectRigidbody = GetComponent<Rigidbody>();
        targetCollider = GetComponent<Collider>();
    }

    public void Grab(Transform objectGrabPointTransfrom){
        this.objectGrabPointTransfrom = objectGrabPointTransfrom;
        objectRigidbody.useGravity = false;
        objectRigidbody.isKinematic = true;
        targetCollider.enabled = false;
    }

    public void Drop(Transform objectGrabPointTransfrom){
        this.objectGrabPointTransfrom = null;
        objectRigidbody.useGravity = true;
        objectRigidbody.isKinematic = false;
        targetCollider.enabled = true;

        // Calculate momentum TO THROW THE OBJECT
        momentum = (objectRigidbody.position - previousPosition) / Time.deltaTime;

        // Decrease momentum based on the mass of the object
        momentum /= objectRigidbody.mass;

        /*
        //average out the momentum for it to be more smooth
        float weight1 = 0.5f;  // Weight for vector1
        float weight2 = 0.5f;  // Weight for vector2

        finalMomentum = finalMomentum * weight1 + momentum * weight2;
        */
        // Apply momentum to the dropped item
        objectRigidbody.velocity = momentum;
    }

    private void Update(){
        if(objectGrabPointTransfrom != null){
            // Store previous position for next frame
            previousPosition = objectRigidbody.position;
            
            objectRigidbody.MovePosition(objectGrabPointTransfrom.position);
        }
    }

   

    public void OnGrab(MonoBehaviour playerThatInteracted)
    {
        Grab(playerThatInteracted.GetComponent<PlayerMovementAdvanced>().ObjectGrabPointTransfrom);
    }

    public void OnDrop(MonoBehaviour playerThatInteracted)
    {
       Drop(playerThatInteracted.GetComponent<PlayerMovementAdvanced>().ObjectGrabPointTransfrom);
    }
}

