using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private PlayerMovementAdvanced playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WaterTrigger")
        {
            // Change state to swimming
            if (playerMovement != null)
            {
                playerMovement.state = PlayerMovementAdvanced.MovementState.swimming;
                // You can add additional logic specific to entering water
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WaterTrigger")
        {
            // Change state to walking
            if (playerMovement != null)
            {
                playerMovement.state = PlayerMovementAdvanced.MovementState.walking;
                // You can add additional logic specific to exiting water
            }
        }
    }
}
