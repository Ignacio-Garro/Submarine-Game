using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SubmarineSinking : MonoBehaviour
{

    [SerializeField] private bool submarineSinking;
    [SerializeField] private bool waterBeingDrained;
    [SerializeField] private float drainageSpeed;
    [SerializeField] private float sinkingLevelOfWater;
    [SerializeField] private int numberOfHolesSinking;
    [SerializeField] private float levelRisingAmountPerHole;
    [SerializeField] private Transform WaterMassThatRisses;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Update sinkinglevel of Water
        if(numberOfHolesSinking > 0){
            submarineSinking = true;

            if(sinkingLevelOfWater < 1){
                sinkingLevelOfWater = sinkingLevelOfWater + numberOfHolesSinking * levelRisingAmountPerHole * Time.deltaTime;
            }
        }
        else{
            submarineSinking = false;
        }

        if(waterBeingDrained && sinkingLevelOfWater > 0){
            sinkingLevelOfWater = sinkingLevelOfWater - drainageSpeed * Time.deltaTime;
        }
        

        // Map input value to a position between start and end positions
        Vector3 targetPosition = Vector3.Lerp(startPosition.position, endPosition.position, sinkingLevelOfWater);

        // Move the object to the target position
        transform.position = targetPosition;
    }
}
