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

    [Header("Holes")]
    private SinkingHoles[] holes;
    private SinkingHoles hole;
    [SerializeField] private SinkingHoles Hole1;
    [SerializeField] private SinkingHoles Hole2;
    [SerializeField] private SinkingHoles Hole3;
    [SerializeField] private SinkingHoles Hole4;


    // Start is called before the first frame update
    void Start()
    {
        // Store the holes in an array for easier access
        holes = new SinkingHoles[] { Hole1, Hole2, Hole3, Hole4 };
        // Start the coroutine to randomly choose a hole every 30 seconds
        StartCoroutine(NewRandomLeak());
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

    private IEnumerator NewRandomLeak()
{
    while (true)
    {
        bool closedHoleFound = false;

        while (!closedHoleFound)
        {
            // Choose a random index between 0 and 3 
            int randomHoleIndex = Random.Range(0, holes.Length);

            hole = holes[randomHoleIndex];

            // Check if the hole is not open
            if (!hole.HoleIsOpen)
            {
                // Open the hole
                hole.TurnOnParticleSystem();
                closedHoleFound = true; // Set the flag to true to exit the loop
                numberOfHolesSinking++;
                Debug.Log("New Hole: " + (randomHoleIndex + 1) + " " + hole);
            }
        }
        closedHoleFound = false;
        // Wait for 30 seconds
        yield return new WaitForSeconds(10f);
    }
}
}
