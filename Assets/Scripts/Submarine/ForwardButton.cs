using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class ForwardButton : MonoBehaviour, IInteractuableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 
    [SerializeField] Engine engine;

    private bool pressed = false;

    public void OnInteract(GameObject playerThatInteracted)
    {
        if(pressed)
        {
            mobileSubmarine.SetForwardMovement(false);
            engine.ChangeEngineStatue(false);
        }
        else
        {
            mobileSubmarine.SetForwardMovement(true);
            engine.ChangeEngineStatue(true);
        }
        pressed = !pressed;
    }

    public void setPressed(bool set){
        pressed = set;
    }

}
