using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class ObjectGrabbable : MonoBehaviour, IGrabbableObject
{
    private Rigidbody objectRigidBody;
    private Transform objectGrabPointTransform;
    private Collider objectCollider;

    private Quaternion originalRotation; // Store the original rotation

    private Transform objectParent;
    private GameObject objectGrabPointTransfromParent;
    private Rigidbody newRigidbody;
    private GameObject obj;


    private void Awake() {
        objectRigidBody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
        originalRotation = Quaternion.identity;
        obj = gameObject;

    }
    private void Grab(Transform objectGrabPointTransform){
        objectParent = transform.parent; // Store the current parent
        objectGrabPointTransfromParent = GameObject.Find("ObjectGrabPoint");
        transform.parent = objectGrabPointTransform.transform;// Make childObject a child of parentObject

        transform.position = objectGrabPointTransform.position;

        this.objectGrabPointTransform = objectGrabPointTransform;

        Rigidbody newRigidbody = objectRigidBody;
        Destroy(obj.GetComponent<Rigidbody>());
        /*
        objectRigidBody.useGravity = false;
        objectRigidBody.Sleep();
        objectRigidBody.detectCollisions = false;
        objectRigidBody.isKinematic = true;
        objectCollider.enabled = false;
        */
    }
    private void Drop(Transform objectGrabPointTransform){
        transform.parent = objectParent;

        this.objectGrabPointTransform = null;

        obj.AddComponent<Rigidbody>();
        //then copy the values from newRigidbody

        /*
        objectRigidBody.useGravity = true;
        objectRigidBody.WakeUp();
        objectRigidBody.detectCollisions = true;
        objectRigidBody.isKinematic = false;
        objectCollider.enabled = true;
        */
    }

    private void Update() {
        /*
        if(objectGrabPointTransform != null){
            float lerpSpeed = 20f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidBody.MovePosition(newPosition);

            objectRigidBody.MoveRotation(Quaternion.Lerp(transform.rotation, originalRotation, Time.deltaTime * 5f)); // Adjust the speed as needed
        }
        */
        if(objectGrabPointTransform != null){
            objectRigidBody.position = objectGrabPointTransform.position;
        }
    }
   

    public void OnGrab(MonoBehaviour playerThatInteracted)
    {
        Grab(playerThatInteracted.GetComponent<PlayerMovement>().ObjectGrabPointTransfrom);
    }

    public void OnDrop(MonoBehaviour playerThatInteracted)
    {
       Drop(playerThatInteracted.GetComponent<PlayerMovement>().ObjectGrabPointTransfrom);
    }
}

