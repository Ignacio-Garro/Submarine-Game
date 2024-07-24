using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class LeftButton : MonoBehaviour, IInteractuableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 

    private bool pressed = false;

    public void OnInteract(GameObject playerThatInteracted)
    {
        if(pressed)
        {
            mobileSubmarine.SetRightMovement(false);
        }
        else
        {
            mobileSubmarine.SetRightMovement(true);
        }
        pressed = !pressed;
    }

    public void setPressed(bool set){
        pressed = set;
    }
    
}
