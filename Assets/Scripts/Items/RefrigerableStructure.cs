using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class RefrigerableStructure : NetworkBehaviour, ItemInteractuableInterface
{
    [SerializeField] int refrigerRequired = 10;
    public Action<float> RefrigerateServerAction = _ => { };
    public Action<float> RefrigerateClientAction = _ => { };

    public void OnEnterInteractionRange(ItemPickable item)
    {
        
    }

    public void OnInteract(ItemPickable item)
    {
        RefrigerateFunction refrigerateItem = item.GetComponent<RefrigerateFunction>();
        if (refrigerateItem == null || refrigerateItem.usesLeft == 0) return;
        RefrigerateServerRpc((float)refrigerateItem.RefrigerPower / refrigerRequired);
    }

    [ServerRpc(RequireOwnership = false)]
    void RefrigerateServerRpc(float refrigerPercent)
    {
        RefrigerateServer(refrigerPercent);
        RefrigerateClientRpc(refrigerPercent);
    }

    [ClientRpc(RequireOwnership = false)]
    void RefrigerateClientRpc(float refrigerPercent)
    {
        RefrigerateClient(refrigerPercent);
    }

    public void RefrigerateServer(float refrigerPercent) {
        RefrigerateServerAction(refrigerPercent);
    }
    public void RefrigerateClient(float refrigerPercent) {
        RefrigerateClientAction(refrigerPercent);
    }
  
    public void OnStopInteracting(ItemPickable item)
    {
        
    }
}
