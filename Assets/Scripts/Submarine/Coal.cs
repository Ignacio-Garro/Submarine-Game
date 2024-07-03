using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coal : MonoBehaviour
{
    [SerializeField] private Engine engine;
    [SerializeField] private int refillFuelAmount;


    public void AddFuelToEngineCoal(){
        engine.RefillEnginefuel(refillFuelAmount);
    } 
}
