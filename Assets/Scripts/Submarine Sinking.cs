using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SubmarineSinking : MonoBehaviour
{

    [SerializeField] private bool submarineSinking;
    [SerializeField] private bool waterBeingDrained;
    [SerializeField] private bool automaticTimedSinking;
    [SerializeField] private float drainageSpeed;
    [SerializeField] private float sinkingLevelOfWater;
    [SerializeField] private int numberOfHolesSinking;
    [SerializeField] private float levelRisingAmountPerHole;
    [SerializeField] private Transform WaterMassThatRisses;
    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;

    [Header("Holes")]
    [SerializeField] private float timerNewHole;
    private SinkingHoles[] holes;
    private SinkingHoles hole;

    // Start is called before the first frame update
    void Start()
    {
        // Store the holes in an array for easier access
        holes = GameObject.FindObjectsOfType<SinkingHoles>();
        // Start the coroutine to randomly choose a hole every 30 seconds
        StartCoroutine(StartRandomLeakAfterDelay());
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
        WaterMassThatRisses.transform.position = targetPosition;
    }

    private IEnumerator StartRandomLeakAfterDelay()
{
    // Wait for 7 seconds before starting the coroutine
    yield return new WaitForSeconds(timerNewHole);

    // Start the NewRandomLeak coroutine
    StartCoroutine(NewRandomLeak());
}
    private IEnumerator NewRandomLeak(){
        while (automaticTimedSinking){
            bool closedHoleFound = false;

            while (!closedHoleFound && numberOfHolesSinking < holes.Length){
                // Choose a random index between 0 and 3 
                int randomHoleIndex = Random.Range(0, holes.Length);
                hole = holes[randomHoleIndex];

                // Check if the hole is not openS
                if (!hole.HoleIsOpen){
                    hole.TurnOnParticleSystem();
                    closedHoleFound = true; // Set the flag to true to exit the loop
                }
            }
            // Wait for x seconds
            yield return new WaitForSeconds(timerNewHole);
        }
    }

    public void ChangeNumberOfHolesSinking(int change){
        numberOfHolesSinking += change;
        if(numberOfHolesSinking < 0){
            numberOfHolesSinking = 0;
        }
    }
}
