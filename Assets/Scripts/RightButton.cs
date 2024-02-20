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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
