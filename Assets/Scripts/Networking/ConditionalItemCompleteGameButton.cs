using NUnit.Framework;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;

public class ConditionalItemCompleteGameButton : NetworkBehaviour, IInteractuableObject
{

    public List<GameObject> acceptableGameObjects;
    public List<string> acceptableTags;

    public UnityEvent<GameObject, ItemPickable> onClickLocal = null;
    public UnityEvent<GameObject, ItemPickable> onClickServer = null;
    public UnityEvent<GameObject, ItemPickable> onClickClients = null;


    public void OnInteract(GameObject playerThatInteracted)
    {
        PlayerInventory inventory = playerThatInteracted.GetComponent<PlayerInventory>();
        if (inventory == null) return;
        if (inventory.currentHoldingItem == null) return;
        ItemPickable item = inventory.currentHoldingItem;
        if (acceptableGameObjects.Contains(PrefabUtility.GetCorrespondingObjectFromSource(inventory.currentHoldingItem.gameObject)) || acceptableTags.Contains(inventory.currentHoldingItem.gameObject.tag))
        {
                onClickLocal.Invoke(playerThatInteracted, item);
                ClickServerRpc(playerThatInteracted, item.gameObject);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClickServerRpc(NetworkObjectReference playerThatInteracted, NetworkObjectReference item)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        item.TryGet(out NetworkObject itemObject);
        onClickServer.Invoke(networkObject == null ? null : networkObject.gameObject, itemObject == null ? null : itemObject.GetComponent<ItemPickable>());
        ClickClientRpc(playerThatInteracted, item);
    }

    [ClientRpc(RequireOwnership = false)]
    public void ClickClientRpc(NetworkObjectReference playerThatInteracted, NetworkObjectReference item)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        item.TryGet(out NetworkObject itemObject);
        onClickClients.Invoke(networkObject == null ? null : networkObject.gameObject, itemObject == null ? null : itemObject.GetComponent<ItemPickable>());
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
