using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class ForwardButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 


    bool pressed = false;
    public void OnClick(MonoBehaviour playerThatClicked)
    {
        if(pressed)
        {
            mobileSubmarine.SetForwardMovement(false);
        }
        else
        {
            mobileSubmarine.SetForwardMovement(true);
        }
        pressed = !pressed;
    }

    public void OnClickRelease()
    {
        
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}