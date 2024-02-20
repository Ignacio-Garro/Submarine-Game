using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransfrom;

    // Store previous position for velocity calculation
    private Vector3 previousPosition;
    [SerializeField]private Vector3 momentum;
    [SerializeField]private Vector3 finalMomentum;


    private void Awake(){
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransfrom){
        this.objectGrabPointTransfrom = objectGrabPointTransfrom;
        objectRigidbody.useGravity = false;
    }

    public void Drop(Transform objectGrabPointTransfrom){
        this.objectGrabPointTransfrom = null;
        objectRigidbody.useGravity = true;

        // Calculate momentum TO THROW THE OBJECT
        momentum = (objectRigidbody.position - previousPosition) / Time.deltaTime;

        // Decrease momentum based on the mass of the object
        momentum /= objectRigidbody.mass;


        //average out the momentum for it to be more smooth
        float weight1 = 0.5f;  // Weight for vector1
        float weight2 = 0.5f;  // Weight for vector2

        finalMomentum = finalMomentum * weight1 + momentum * weight2;
        // Apply momentum to the dropped item
        objectRigidbody.velocity = finalMomentum;
    }

    private void FixedUpdate(){
        if(objectGrabPointTransfrom != null){
            // Store previous position for next frame
            previousPosition = objectRigidbody.position;

            float lerpSpeed = 20f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransfrom.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(newPosition);
        }
    }
}

