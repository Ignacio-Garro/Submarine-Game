using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SinkingHole : NetworkBehaviour
{
    [SerializeField] private bool holeIsOpen = false;

    // Getter for holeIsOpen
    public bool HoleIsOpen => holeIsOpen;
    
    private ParticleSystem waterParticles;
    public Action turnOnServerCallback = () => { };
    public Action turnOffServerCallback = () => { };

    

    public override void OnNetworkSpawn()
    {
        waterParticles = GetComponentInChildren<ParticleSystem>();
        TurnOffParticleSystem();
    }

    public void TurnOnParticleSystem()
    {
        if (IsClient)
        {
            waterParticles.gameObject.SetActive(true); // tengo que volver a abilitar el gameobject
            waterParticles.Play(); // Start emitting particles
        }
        if (!HoleIsOpen && IsServer)
        {
            turnOnServerCallback();
        }
        holeIsOpen = true;

    }

    // Method to turn off the Particle System
    public void TurnOffParticleSystem()
    {
        if(IsClient)
        {
            waterParticles.Stop();
        }
        if (IsServer && holeIsOpen)
        {
            turnOffServerCallback();
        }
        holeIsOpen = false; 
        
    }

    [ClientRpc(RequireOwnership = false)]
    public void TurnOnParticleSystemClientRpc()
    {
        TurnOnParticleSystem();
    }

    public void OpenHoleFromServer()
    {
        TurnOnParticleSystem();
        TurnOnParticleSystemClientRpc();
    }

   
}
