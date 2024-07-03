using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkingHoles : MonoBehaviour
{
    [SerializeField] private bool holeIsOpen;

    // Getter for holeIsOpen
    public bool HoleIsOpen
    {
        get { return holeIsOpen; }
    }
    private ParticleSystem waterParticles;
    private SubmarineSinking submarineSinking;

    private void Start(){
        waterParticles = GetComponentInChildren<ParticleSystem>();
        submarineSinking = GetComponentInParent<SubmarineSinking>();
        TurnOffParticleSystem();
    }

    public void TurnOnParticleSystem()
    {
        holeIsOpen = true;
        waterParticles.gameObject.SetActive(true); // tengo que volver a abilitar el gameobject
        submarineSinking.ChangeNumberOfHolesSinking(1);
        waterParticles.Play(); // Start emitting particles
    }

    // Method to turn off the Particle System
    public void TurnOffParticleSystem()
    {
        holeIsOpen = false; 
        submarineSinking.ChangeNumberOfHolesSinking(-1);
        waterParticles.Stop(); // Stop emitting particles - wtf me hace disable de el gameobject
        
    }
    
}
