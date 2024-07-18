using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickable : MonoBehaviour, IInteractuableObject
{
    public ItemSO ItemScriprableObject;

    //On interact picks the item
    public void OnInteract(GameObject playerThatInteracted)
    {
        PickItem(playerThatInteracted);
    }

    public void PickItem(GameObject playerThatInteracted)
    {
        PlayerInventory inventory = playerThatInteracted.GetComponent<PlayerInventory>();
        if (inventory == null) return;
        inventory.PickupObject(this);
        Destroy(gameObject);
    }
}

