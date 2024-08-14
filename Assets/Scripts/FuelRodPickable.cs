using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelRodPickable : ItemPickable
{
    public override void PickItem(GameObject playerThatInteracted)
    {
        FuelRod fuelObject = GetComponent<FuelRod>();
        if(fuelObject != null)
        {
            fuelObject.RemoveFromReactor();
        }
        base.PickItem(playerThatInteracted);
    }
}

