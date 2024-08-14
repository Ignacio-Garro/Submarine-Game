using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ItemPickable : NetworkBehaviour, IInteractuableObject
{
    public Sprite inventoryImage;
    public PlayerInventory currentInventory = null;
    public GameObject CurrentPlayer => GameManager.Instance.ActualPlayer;
    public PlayerInventory CurrentPlayerInventory => CurrentPlayer.GetComponent<PlayerInventory>();
    public bool IsBeingHold = false;

    //On interact picks the item
    public void OnInteract(GameObject playerThatInteracted)
    {
        PickItem(playerThatInteracted);
    }

    public virtual void PickItem(GameObject playerThatInteracted)
    {
        PlayerInventory inventory = playerThatInteracted.GetComponent<PlayerInventory>();
        if (inventory == null) return;
        inventory.PickupObject(this);
    }

    private void Update()
    {
        if(IsOwner && IsBeingHold)
        {
            if(CurrentPlayerInventory != null)
            {
                transform.position = CurrentPlayerInventory.ObjectInventoryPosition.position;
                transform.rotation = CurrentPlayerInventory.ObjectInventoryPosition.rotation;
            }
        }
    }
}

