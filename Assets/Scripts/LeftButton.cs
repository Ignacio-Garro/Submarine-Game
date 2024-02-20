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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
