using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixAreaHole : MonoBehaviour, IClickableObject
{
    private SinkingHoles sinkingHoles;

    private void Start() {
        sinkingHoles = GetComponentInParent<SinkingHoles>();
    }

    public void OnClick(MonoBehaviour playerThatClicked){
        Debug.Log("1");
        if(sinkingHoles.HoleIsOpen){
            Debug.Log("2");
            sinkingHoles.TurnOffParticleSystem();
        }
    }

    public void OnClickRelease(){
    }
}
