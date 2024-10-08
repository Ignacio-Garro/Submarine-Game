using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BurstEnergyItemFunction : EnergyItemFunction
{
    [SerializeField] float consumptionBurst = 100000f;


    [ServerRpc(RequireOwnership = false)]
    void ReduceEnergyServerRpc(ulong id)
    {
        currentEnergy.Value -= consumptionBurst;
    }

    public abstract void Burst(GameObject interactingObject);

    public void OnItemRemove()
    {
        
    }

    public void OnItemUnuse()
    {

    }

    public override void OnItemUse(GameObject interactingObjects)
    {
        if (currentEnergy.Value <= consumptionBurst) return;
        ReduceEnergyServerRpc(GameManager.Instance.ActualPlayer.GetComponent<NetworkObject>().NetworkObjectId);
        Burst(interactingObjects);
    }

    public override void OnItemUnuse(GameObject interactingObjects) { }
    public override void OnItemRemove(GameObject interactingObjects) { }
    public override void OnItemOutOfView(GameObject interactingObjects) { }
    public override void OnItemInView(GameObject interactingObjects) { }

}
