using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EnergyItemFunction : NetworkBehaviour, ItemFunctionInterface
{
    [SerializeField]
    protected float energyCapacity = 300000f;
    protected NetworkVariable<float> currentEnergy = new NetworkVariable<float>(0);
    public ChargeStation currentChargeStation = null;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer) currentEnergy.Value = energyCapacity;
        ItemPickable item = GetComponent<ItemPickable>();
        if (item != null) item.OnPickItem += _ => ExtractFromChargeStationServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    public void ExtractFromChargeStationServerRpc()
    {
        if(currentChargeStation != null) currentChargeStation.ExtractCurrentItem();
    }

    public void Charge(float energyAmmount)
    {
        currentEnergy.Value += energyAmmount;
    }

    public float GetRemainingMaxCharge()
    {
        return energyCapacity - currentEnergy.Value;
    }

    public float GetRemainingEnergyPercentage()
    {
        return currentEnergy.Value / energyCapacity;
    }

    public abstract void OnItemUse(GameObject interactingObjects);
    public abstract void OnItemUnuse(GameObject interactingObjects);
    public abstract void OnItemRemove(GameObject interactingObjects);
    public abstract void OnItemOutOfView(GameObject interactingObjects);
    public abstract void OnItemInView(GameObject interactingObjects);
}
