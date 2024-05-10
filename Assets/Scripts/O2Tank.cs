using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class O2Tank : MonoBehaviour, IClickableObject
{
    [SerializeField] private O2TankGrabbable o2TankGrabbable;
    [SerializeField] private PlayerStats playerStats;
    //GasTankGrab
    public void OnClick(MonoBehaviour playerThatClicked)
    {
        // If the grabbed object is an O2TankGrabbable
        if (playerStats.getGrabbedObject() != null && playerStats.getGrabbedObject().GetComponent<O2TankGrabbable>()){
            o2TankGrabbable.O2GoesInTank();
        }
    }

    public void OnClickRelease()
    {

    }
}
