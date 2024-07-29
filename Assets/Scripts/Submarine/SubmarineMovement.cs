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

    [Header("Physic attributes")]
    [SerializeField] float submarineMass = 2100000f;
    [SerializeField] float submarineInsideVolumeLitres = 1900000f;
    [SerializeField] float submarineTankVolumeLitres = 515000f;
    [SerializeField] float diamenter = 10f;
    [SerializeField] float length = 100f;
    [SerializeField] float tankPercentage = 0f;
    [SerializeField] float gravity = 9.8f;

    [Header("Engine Attributes")]
    [SerializeField] float maxEnginePowerWatt = 2500000f;
    [SerializeField] float currentPowerPercent = 0;

    [Header("HorizontalMovement")]
    [SerializeField] float horizontalDragCoeficient = 1f;
    [SerializeField] float horizontalVelocity = 0f;
    
    float horizontalSurface => diamenter * diamenter / 4 * Mathf.PI;

    float currentPowerWatt => maxEnginePowerWatt * currentPowerPercent / 100;


    [Header("Vertical movement")]
    [SerializeField] float verticalDragCoeficient = 1f;
    [SerializeField] float verticalVelocity = 0;
    [SerializeField] float waterHeigth = 0;
    [SerializeField] float submarineHeigth = 5f;
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
        
        Vector3 rotation = Vector3.up * rotateVelocity * Time.fixedDeltaTime;
        // Apply rotation to the transform
        transform.Rotate(rotation);
        HorizontalMovement();
        VerticalMovement();
    }

    public void HorizontalMovement()
    {
        float horizontalForce = currentPowerWatt / Mathf.Max(Mathf.Abs(horizontalVelocity),0.1f);
        float horizontalDrag = Mathf.Sign(-horizontalVelocity) * horizontalDragCoeficient * 0.5f * 1000 * horizontalSurface * horizontalVelocity * horizontalVelocity;
        float totalForce = horizontalForce + horizontalDrag;
        bool dragChangedForceDirection = Mathf.Sign(totalForce) != Mathf.Sign(horizontalForce);
        float acceleration = totalForce / totalMass;
        if(dragChangedForceDirection)
        {
            horizontalVelocity = Mathf.Sign(horizontalVelocity + acceleration * Time.fixedDeltaTime) == Mathf.Sign(horizontalVelocity) ? horizontalVelocity + acceleration * Time.fixedDeltaTime : 0;
        }
        else
        {
            horizontalVelocity += acceleration * Time.fixedDeltaTime;
        }
        transform.position += transform.forward * horizontalVelocity * Time.fixedDeltaTime;
    }

    void VerticalMovement()
    {
        float weigthForce = totalMass * gravity;
        float bouyancyForce = underWaterVolume * gravity;
        //We fix a minimun drag velocity so the submarine doesnt brake that slowly at very low velociies
        float fixeddragVelocity = (verticalVelocity < minDragVelocity && verticalVelocity > -minDragVelocity) ? minDragVelocity : verticalVelocity;
        float dragForce = Mathf.Sign(-verticalVelocity) * verticalDragCoeficient * 0.5f * 1000 * verticalSurface * fixeddragVelocity * fixeddragVelocity;
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
        currentPowerPercent = 100;
    }
    public void StopForwandMovement()
    {
        currentPowerPercent = 0;
    }
    public void StopBackWardsMovement()
    {
        currentPowerPercent = 0;
    }
    public void StarBackWardsMovement()
    {
        currentPowerPercent = -100;
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
