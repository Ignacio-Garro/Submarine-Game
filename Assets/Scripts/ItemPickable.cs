using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;

public class ItemPickable : NetworkBehaviour, IInteractuableObject
{
    public Sprite inventoryImage;
    public PlayerInventory currentInventory = null;
    public GameObject CurrentPlayer => GameManager.Instance.ActualPlayer;
    public PlayerInventory CurrentPlayerInventory => CurrentPlayer.GetComponent<PlayerInventory>();
    public bool IsBeingHold = false;
    public Action<GameObject> OnPickItem = _ => { };

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
        OnPickItem(playerThatInteracted);
    }


    private void Update()
    {
        //Move to object position only if the player is the owner, and if the player is holding the item
        //Can be optimized
        if(IsOwner && IsBeingHold)
        {
            if(CurrentPlayerInventory != null)
            {
                transform.position = CurrentPlayerInventory.ObjectInventoryPosition.position;
                transform.rotation = CurrentPlayerInventory.ObjectInventoryPosition.rotation;
            }
        }
    }



    public void ChangeItemProperty(PlayerInventory inventory)
    {
        NetworkObject nObj = GetComponent<NetworkObject>();
        //if the previous owner is different from the new one remove the object form the formers inventory
        if(NetworkManager.Singleton.LocalClientId != nObj.OwnerClientId)
        {
            RemovePreviousOwnerServerRpc(nObj.OwnerClientId);
            NetworkCommunicationManager.Instance.ChangeOwnerShipServerRpc(gameObject, NetworkManager.Singleton.LocalClientId);
        }
        currentInventory = inventory;
    }

    [ServerRpc(RequireOwnership = false)]
    public void RemovePreviousOwnerServerRpc(ulong previousOwnerId)
    {
        RemovePreviousOwnerClientRpc(previousOwnerId);
    }
    [ClientRpc(RequireOwnership = false)]
    public void RemovePreviousOwnerClientRpc(ulong previousOwnerId)
    {
        if (NetworkManager.Singleton.LocalClientId == previousOwnerId && IsBeingHold && currentInventory != null)
        {
            currentInventory.ExtractItemForcefully(this);
        }
    }

    public void OnEnterInRange()
    {
        InputManager.Instance.AddInteractuableMaterial(gameObject);
    }

    public void OnExitInRange()
    {
        InputManager.Instance.RemoveInteractuableMaterial(gameObject);
    }

}

