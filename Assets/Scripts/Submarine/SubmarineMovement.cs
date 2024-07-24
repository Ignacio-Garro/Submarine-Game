using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using Unity.Netcode;
using UnityEngine;

public class SubmarineMovement : NetworkBehaviour
{
    [Header("sub nav controls")]
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float forwardsAcceleration = 1;
    [SerializeField] float backWardsAcceleration = 2;
    [SerializeField] float Deceleration = 1;

    
    [SerializeField] float rotationAcceleration = 8f;
    [SerializeField] float maxRotateVelocity = 25;
    [SerializeField] float rotationDeceleration = 5f;

    [SerializeField] bool workingEngine;

    [Header("nav info")]
    [SerializeField] bool isMovingForward;
    [SerializeField] bool isMovingBackWards;
    [SerializeField] bool isMovingRight;
    [SerializeField] bool isMovingLeft;

   
    
    float velocity = 0;
    float rotateVelocity = 0;
    


    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        if (isMovingForward && !isMovingBackWards) {
            velocity = Mathf.Min(velocity + forwardsAcceleration * Time.deltaTime, maxVelocity);
        }
        else if (!isMovingForward && isMovingBackWards) {
            velocity = Mathf.Max(velocity - backWardsAcceleration * Time.deltaTime, -maxVelocity);
        }
        else {
            velocity = Mathf.MoveTowards(velocity, 0, Deceleration * Time.deltaTime);
        }
        if(isMovingRight) {
            rotateVelocity = Mathf.Min(rotateVelocity + rotationAcceleration * Time.deltaTime, maxRotateVelocity);
        }
        else if(isMovingLeft) {
            rotateVelocity = Mathf.Max(rotateVelocity - rotationAcceleration * Time.deltaTime, -maxRotateVelocity);
        }
        else {
            rotateVelocity = Mathf.MoveTowards(rotateVelocity, 0, rotationDeceleration * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        if (!IsServer) return;
        transform.position += transform.forward * velocity * Time.fixedDeltaTime;
        Vector3 rotation = Vector3.up * rotateVelocity * Time.fixedDeltaTime;
        // Apply rotation to the transform
        transform.Rotate(rotation);
    }


  
    public void SetworkingEngine(bool engineState){
        if(!engineState){
            isMovingForward = false;
            isMovingBackWards = false;
            isMovingRight = false;
            isMovingLeft = false;
        }
        workingEngine = engineState;
    }


    public void StartForwandMovement()
    {
        isMovingForward = true;
    }
    public void StopForwandMovement()
    {
        isMovingForward = false;
    }
    public void StopBackWardsMovement()
    {
        isMovingBackWards = false;
    }
    public void StarBackWardsMovement()
    {
        isMovingBackWards = true;
    }
    public void StopRightMovement()
    {
        isMovingRight = false;
    }
    public void StartRightMovement()
    {
        isMovingRight = true;
    }
    public void StartLeftMovement()
    {
        isMovingLeft = true;
    }
    public void StopLeftMovement()
    {
        isMovingLeft = false;
    }

}
