using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float acceleration = 10;

    Rigidbody rigidBody;
    bool isMovingForward = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        rigidBody.maxLinearVelocity = maxVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        if(isMovingForward)
        {
            rigidBody.AddForce(gameObject.transform.forward * rigidBody.mass * acceleration);
        }

    }

    void StartForwardMovement()
    {

    }

}
