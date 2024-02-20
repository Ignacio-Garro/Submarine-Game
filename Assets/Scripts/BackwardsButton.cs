using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class BackwardsButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 


    bool pressed = false;
    public void OnClick(MonoBehaviour playerThatClicked)
    {
        if(pressed)
        {
            mobileSubmarine.SetBackWardsMovement(false);
        }
        else
        {
            mobileSubmarine.SetBackWardsMovement(true);
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
