using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class BackwardsButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 
    [SerializeField] Engine engine;


    private bool pressed = false;
    public void OnClick(GameObject playerThatInteracted)
    {
        if(pressed)
        {
            mobileSubmarine.SetBackWardsMovement(false);
            engine.ChangeEngineStatue(false);
        }
        else
        {
            mobileSubmarine.SetBackWardsMovement(true);
            engine.ChangeEngineStatue(true);
        }
        pressed = !pressed;
    }

    public void OnClickRelease()
    {
        
    }

    public void setPressed(bool set){
        pressed = set;
    }
}
