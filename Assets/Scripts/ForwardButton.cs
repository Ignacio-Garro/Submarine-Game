using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class ForwardButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 
    [SerializeField] Engine engine;


    bool pressed = false;
    public void OnClick(MonoBehaviour playerThatClicked)
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

    public void OnClickRelease()
    {
        
    }

}
