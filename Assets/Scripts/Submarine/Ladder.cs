using System;
using UnityEngine;
using UnityEngine.Events;

public class Ladder : MonoBehaviour
{
    [SerializeField] private string tagFilter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagFilter))
        {
            Rigidbody playerRb = other.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false; 
                playerRb.linearVelocity = Vector3.zero;
                other.GetComponentInParent<PlayerMovement>().IsInLadder(true);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(tagFilter))
        {
            Rigidbody playerRb = other.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = true; 
                other.GetComponentInParent<PlayerMovement>().IsInLadder(false); 
            }
        }
    }
}
