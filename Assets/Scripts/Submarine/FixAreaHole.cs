using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAreaHole : MonoBehaviour
{
    private SinkingHole sinkingHoles;

    private void Start() {
        sinkingHoles = GetComponentInParent<SinkingHole>();
    }

    public void OnClick(GameObject playerThatInteracted)
    {
        Debug.Log("1");
        if(sinkingHoles.HoleIsOpen){
            Debug.Log("2");
            sinkingHoles.TurnOffParticleSystem();
        }
    }

    public void OnClickRelease(){
    }
}
