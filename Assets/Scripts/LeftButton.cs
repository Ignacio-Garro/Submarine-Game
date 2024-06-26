using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RightButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 


    
    public void OnClick(MonoBehaviour playerThatClicked)
    {
        mobileSubmarine.SetRightMovement(true);
    }

    public void OnClickRelease()
    {
        mobileSubmarine.SetRightMovement(false);
    }
}
