using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RightButton : MonoBehaviour, IInteractuableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 

    private bool pressed = false;

    public void OnInteract(GameObject playerThatInteracted)
    {
        if(pressed)
        {
            mobileSubmarine.SetLeftMovement(false);
        }
        else
        {
            mobileSubmarine.SetLeftMovement(true);
        }
        pressed = !pressed;
    }

    public void setPressed(bool set){
        pressed = set;
    }
}
