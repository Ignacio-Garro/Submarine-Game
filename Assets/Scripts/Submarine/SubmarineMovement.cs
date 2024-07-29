using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SubmarineMovement : NetworkBehaviour
{
    [Header("Conmtroller")]
    [SerializeField] SubmarineController controller;

    [Header("sub nav controls")]
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float forwardsAcceleration = 1;
    [SerializeField] float backWardsAcceleration = 2;
    [SerializeField] float Deceleration = 1;
    [SerializeField] float maxVerticalVelocity = 5;
    [SerializeField] float maxDepth = 20;
    [SerializeField] float rotationAcceleration = 8f;
    [SerializeField] float maxRotateVelocity = 25;
    [SerializeField] float rotationDeceleration = 5f;


    [Header("Vertical movement")]
    [SerializeField] float submarineMass = 2100000f;
    [SerializeField] float submarineInsideVolumeLitres = 1900000f;
    [SerializeField] float submarineTankVolumeLitres = 515000f;
    [SerializeField] float diamenter = 10f;
    [SerializeField] float length = 100f;
    [SerializeField] float tankPercentage = 0f;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float dragCoeficient = 1f;
    [SerializeField] float verticalVelocity = 0;
    [SerializeField] float waterHeigth = 0;
    [SerializeField] float submarineHeigth = 2f;
    [SerializeField] float minDragVelocity = 0.1f;
    float verticalSurface => diamenter * length;
    float totalVolume => submarineInsideVolumeLitres + submarineTankVolumeLitres;
    float underWaterVolume => (1 - Mathf.Clamp((transform.position.y - waterHeigth) / submarineHeigth, 0, 1)) * totalVolume;
    float tankMass => tankPercentage / 100 * submarineTankVolumeLitres;
    float sinkingMass => controller.sinking.WaterLevel / 100 * submarineInsideVolumeLitres;
    float totalMass => submarineMass + tankMass + sinkingMass;
    


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
        VerticalMovement();
    }

    void VerticalMovement()
    {
        float weigthForce = totalMass * gravity;
        float bouyancyForce = underWaterVolume * gravity;
        //We fix a minimun drag velocity so the submarine doesnt brake that slowly at very low velociies
        float fixeddragVelocity = (verticalVelocity < minDragVelocity && verticalVelocity > -minDragVelocity) ? minDragVelocity : verticalVelocity;
        float dragForce = Mathf.Sign(-verticalVelocity) * dragCoeficient * 0.5f * 1000 * verticalSurface * fixeddragVelocity * fixeddragVelocity;
        float totalForce = bouyancyForce - weigthForce + dragForce;
        bool dragChangedForceDirection = Mathf.Sign(totalForce) != Mathf.Sign(bouyancyForce - weigthForce);
        float acceleration = totalForce / totalMass;
        if (dragChangedForceDirection)
        {   
            //We clamp the velocity to assure that the drag never changes the direction of the velocity. Drag should always only subtract force without getting to the point of changing the direction of the object. 
            verticalVelocity = Mathf.Sign(verticalVelocity + acceleration * Time.fixedDeltaTime) == Mathf.Sign(verticalVelocity) ? verticalVelocity + acceleration * Time.fixedDeltaTime : 0;
        }
        else
        {
            verticalVelocity += acceleration * Time.fixedDeltaTime;
        }
        transform.position += transform.up * verticalVelocity * Time.fixedDeltaTime;
    }

    /*
    void OldVerticalMovement()
    {
        float acceleration;
        float targetVerticalVelocity = Mathf.Sign(realTargetHeigth - transform.position.y) * Mathf.Lerp(0, maxVerticalVelocity, Mathf.Abs(realTargetHeigth - transform.position.y) / maxDepth);
        if (verticalVelocity > 0 && targetVerticalVelocity < verticalVelocity || verticalVelocity < 0 && targetVerticalVelocity > verticalVelocity)
        {
            acceleration = 2;
        }
        else
        {
            acceleration = (Mathf.Abs(realTargetHeigth - transform.position.y) / 10);
        }

        verticalVelocity = Mathf.MoveTowards(verticalVelocity, targetVerticalVelocity, acceleration * Time.fixedDeltaTime);
        transform.position += transform.up * verticalVelocity * Time.fixedDeltaTime;
    }
    */

  
    public void SetworkingEngine(bool engineState){
        if(!engineState){
            isMovingForward = false;
            isMovingBackWards = false;
            isMovingRight = false;
            isMovingLeft = false;
        }
        //workingEngine = engineState;
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

    public void IncreaseTankWater()
    {
        tankPercentage = Mathf.Min(tankPercentage + 10, 100);
    }

    public void DecreaseTankWater()
    {
        tankPercentage = Mathf.Max(tankPercentage - 10, 0);
    }

    public void BalanceTankWater()
    {
        float neededMass = totalVolume - submarineMass;
        float neededPercent = neededMass / submarineTankVolumeLitres;
        tankPercentage = neededPercent * 100;
    }
}
