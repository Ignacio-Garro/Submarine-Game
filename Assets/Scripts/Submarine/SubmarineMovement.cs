using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;

public class SubmarineMovement : NetworkBehaviour
{
    [Header("Conmtroller")]
    [SerializeField] SubmarineController controller;

    [Header("sub nav controls")]
    [SerializeField] float rotationAcceleration = 2f;
    [SerializeField] float maxRotateVelocity = 5f;
    [SerializeField] float rotationDeceleration = 2f;
    
 

    [Header("Physic attributes")]
    [SerializeField] float submarineMass = 2100000f;
    [SerializeField] float submarineInsideVolumeLitres = 1900000f;
    [SerializeField] float submarineTankVolumeLitres = 515000f;
    [SerializeField] float diamenter = 10f;
    [SerializeField] float length = 100f;
    [SerializeField] float tankPercentage => 100f - (controller.FloatLever.LeverPosition + 100f) / 2f;

    

    [Header("Engine Attributes")]
    [SerializeField] List<Transform> propellerObject; 
    [SerializeField] float maxEnginePowerWatt = 2500000f;
    
    [SerializeField] float propellerRadius = 5;
    [SerializeField] float propellerMass = 3000f;
    [SerializeField] float propellerEfficiencyPerSecond = 0.8f;
    [SerializeField] float propellerConversionToPushForcePerSecond = 0.1f;
    [SerializeField] float numberOfPropellers = 3;
    float currentPowerPercent => controller.MovementLever.LeverPosition;

    [Header("HorizontalMovement")]
    [SerializeField] float horizontalDragCoeficient = 1f;
    [SerializeField] float horizontalVelocity = 0f;
    [SerializeField] float propellerAngularVelocity = 0f;
    [SerializeField] float minDragHorizontalVelocity = 3f;
    float horizontalSurface => diamenter * diamenter / 4 * Mathf.PI;
    


    [Header("Vertical movement")]
    [SerializeField] float verticalDragCoeficient = 1f;
    [SerializeField] float verticalVelocity = 0;
    [SerializeField] float submarineHeigth = 5f;
    [SerializeField] float minDragVelocity = 0.1f;
    [SerializeField] float waterMassFactor = 0.1f;

    Engine leftEngine => controller.leftEngine;
    Engine rightEngine => controller.rigthEngine;
    float verticalSurface => diamenter * length;
    float totalVolume => submarineInsideVolumeLitres + submarineTankVolumeLitres;
    float underWaterVolume => (1 - Mathf.Clamp((transform.position.y - waterHeigth) / submarineHeigth, 0, 1)) * totalVolume;
    float tankMass => tankPercentage / 100 * submarineTankVolumeLitres;
    float sinkingMass => controller.sinking.WaterLevel / 100 * submarineInsideVolumeLitres * waterMassFactor;
    float totalMass => submarineMass + tankMass + sinkingMass;



    [Header("nav info")]
    [SerializeField] TextMeshProUGUI horizontalSpeedText;
    [SerializeField] TextMeshProUGUI energyText;
    [SerializeField] TextMeshProUGUI verticalSpeedText;
    [SerializeField] TextMeshProUGUI depthText;
    [SerializeField] TextMeshProUGUI pressureText;

    [SerializeField] bool isMovingRight;
    [SerializeField] bool isMovingLeft;
   

    float velocity = 0;
    [SerializeField] float rotateVelocity = 0;


    //Values from controller
    float gravity => controller.SubmarineGravity;
    float waterHeigth => controller.WaterHeigth;


    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        float targetRotateVelocity = (controller.RotationLever.Progress * 2 - 1) * maxRotateVelocity;
        rotateVelocity = Mathf.MoveTowards(rotateVelocity, targetRotateVelocity, rotationAcceleration * Time.deltaTime);
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
        float currentPercentage = (leftEngine.GetMotorUsablePercentage(currentPowerPercent/100f) + rightEngine.GetMotorUsablePercentage(currentPowerPercent/100f)) / 2;
        float currentPowerWatt = maxEnginePowerWatt * currentPercentage;
        float usableEnergy = controller.reactor.TryToExctractEnergy(Mathf.Abs(currentPowerWatt) * Time.fixedDeltaTime);
        if(currentPowerWatt != 0)
        {
            float usedPercentage = usableEnergy / (Mathf.Abs(currentPowerWatt) * Time.fixedDeltaTime);
            leftEngine.UseMotorPercentage(usedPercentage);
            rightEngine.UseMotorPercentage(usedPercentage);
        }
        
        float horizontalForce = Mathf.Sign(currentPowerWatt) * (usableEnergy/Time.fixedDeltaTime) / Mathf.Max(Mathf.Abs(horizontalVelocity),3f); //fuerza que genera el motor
        float fixeddragVelocity = (horizontalVelocity < minDragHorizontalVelocity && horizontalVelocity > -minDragHorizontalVelocity) ? minDragHorizontalVelocity : horizontalVelocity;//
        float horizontalDrag = Mathf.Sign(-horizontalVelocity) * horizontalDragCoeficient * 0.5f * 1000 * horizontalSurface * fixeddragVelocity * fixeddragVelocity;
        if (horizontalVelocity == 0) horizontalDrag = 0;
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
        horizontalSpeedText.text = Math.Round(horizontalVelocity,1) + "<size=40%> kts";
        energyText.text = (usableEnergy / Time.fixedDeltaTime) / 1000f + "<size=40%> Kw";
    }

    void VerticalMovement()
    {
        float weigthForce = totalMass * gravity;
        float bouyancyForce = underWaterVolume * gravity;
        //We fix a minimun drag velocity so the submarine doesnt brake that slowly at very low velociies
        float fixeddragVelocity = (verticalVelocity < minDragVelocity && verticalVelocity > -minDragVelocity) ? minDragVelocity : verticalVelocity;
        float dragForce = Mathf.Sign(-verticalVelocity) * verticalDragCoeficient * 0.5f * 1000 * verticalSurface * fixeddragVelocity * Math.Sign(fixeddragVelocity);
        
        if(verticalVelocity == 0) dragForce = 0;
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
        controller.collision.MakeDownMovementWithCollisions(verticalVelocity * Time.fixedDeltaTime);
        


        //Update screen text
        float depth = (float)Math.Round(-transform.position.y, 1);
        verticalSpeedText.text = Math.Round(verticalVelocity,1) + " m/s";
        depthText.text = depth + " m";
        pressureText.text = CalculatePressure(depth) + " atm";

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
/*        if(!engineState){
            isMovingForward = false;
            isMovingBackWards = false;
            isMovingRight = false;
            isMovingLeft = false;
        }*/
        //workingEngine = engineState;
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

    public static float CalculatePressure(double depth)
    {
        const double densityWater = 1000; // Densidad del agua en kg/m^3
        const double gravity = 9.81; // Gravedad en m/s^2
        const double atmosphericPressure = 1; // Presión atmosférica en atm
        

        double pressureAtDepth = atmosphericPressure + (densityWater * gravity * depth) / 101325;

        return (float)Math.Round(pressureAtDepth, 3); // Convert from pascals to atmospheres
    }

    public void BalanceTankWater()
    {
        float neededMass = totalVolume - submarineMass;
        float neededPercent = neededMass / submarineTankVolumeLitres;
        //tankPercentage = neededPercent * 100;
    }
}
