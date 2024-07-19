using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2TankGrabbable : MonoBehaviour
{
    [SerializeField] private int maxO2LevelsInTank;
    [SerializeField] private int o2LevelsInTank;
    [SerializeField] private int o2InputSpeed;
    [SerializeField] private int o2OutputSpeed;
    

    public void O2GoesInTank(){
        // Check if the tank is not already full
        if (o2LevelsInTank < maxO2LevelsInTank)
        {
            o2LevelsInTank += o2InputSpeed;
        }
    }
    public void O2GoesOutTank(){
        // Check if the tank is not empty
        if (o2LevelsInTank > 0)
        {
            o2LevelsInTank -= o2OutputSpeed;
        }
    }

}
