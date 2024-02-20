using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransfrom;

    private void Awake(){
        objectRigidbody = GetComponent<Rigidbody>();
    }

    public void Grab(Transform objectGrabPointTransfrom){
        this.objectGrabPointTransfrom = objectGrabPointTransfrom;
        objectRigidbody.useGravity = false;
    }

    public void Drop(){
        this.objectGrabPointTransfrom = null;
        objectRigidbody.useGravity = true;
    }

    private void FixedUpdate(){
        if(objectGrabPointTransfrom != null){
            float lerpSpeed = 10f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransfrom.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(objectGrabPointTransfrom.position);
        }
    }
}
