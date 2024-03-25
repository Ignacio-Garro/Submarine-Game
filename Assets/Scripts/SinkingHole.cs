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
    [SerializeField] private ParticleSystem waterParticles;

    private void Start(){
        waterParticles = GetComponentInChildren<ParticleSystem>();
        TurnOffParticleSystem();
    }

    
    public void TurnOnParticleSystem()
    {
        holeIsOpen = true;
        if (waterParticles != null)
        {
            waterParticles.Play(); // Start emitting particles
            Debug.Log("Particels on of: " + waterParticles);
        }
    }

    // Method to turn off the Particle System
    public void TurnOffParticleSystem()
    {
        holeIsOpen = false;
        if (waterParticles != null)
        {
            waterParticles.Stop(); // Stop emitting particles
        }
    }
    
}
