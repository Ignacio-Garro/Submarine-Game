using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;




public class LeftButton : MonoBehaviour, IClickableObject
{
    [SerializeField] SubmarineMovement mobileSubmarine; 


    
    public void OnClick(MonoBehaviour playerThatClicked)
    {
        mobileSubmarine.SetLeftMovement(true);
    }

    public void OnClickRelease()
    {
        mobileSubmarine.SetLeftMovement(false);
    }
}
