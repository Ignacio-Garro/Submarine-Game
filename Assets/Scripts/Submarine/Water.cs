using System;
using UnityEngine;
using UnityEngine.Events;

public class Water : MonoBehaviour
{
    [SerializeField] private string tagFilter;
    [SerializeField] private GameObject WaterFilter;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagFilter))
        {
            Rigidbody playerRb = other.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.useGravity = false; 
                other.GetComponentInParent<PlayerMovement>().IsInWater(true);
                WaterFilter.SetActive(true);
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
                other.GetComponentInParent<PlayerMovement>().IsInWater(false); 
                WaterFilter.SetActive(false);
            }
        }
    }
}
