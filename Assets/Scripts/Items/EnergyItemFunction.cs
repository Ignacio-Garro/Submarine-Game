using Unity.Netcode;
using UnityEngine;

public class EnergyItemFunction : NetworkBehaviour, ItemFunctionInterface
{
    [SerializeField] float energyCapacity = 300000f;
    [SerializeField] float consumptionRate = 1000f;
    NetworkVariable<float> currentEnergy = new NetworkVariable<float>(0);
    protected int currentState = 0;
    protected int stateNumber = 2;
    public ChargeStation currentChargeStation = null;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(IsServer) currentEnergy.Value = energyCapacity;
        ItemPickable item = GetComponent<ItemPickable>();
        if (item != null) item.OnPickItem += _ => ExtractFromChargeStation();
    }

    public void ExtractFromChargeStation()
    {
        if(currentChargeStation != null) currentChargeStation.ExtractCurrentItem();
    }


    public void OnItemUse()
    {
        if (currentEnergy.Value <= 0) return;
        int nextState = (currentState + 1) % stateNumber;
        switchState(nextState); 
        switchStateServerRpc(nextState, GameManager.Instance.ActualPlayer.GetComponent<NetworkObject>().NetworkObjectId);
    }

    protected virtual void switchState(int state)
    {
        currentState = state;
    }

    [ServerRpc(RequireOwnership = false)]
    void switchStateServerRpc(int state, ulong id)
    {
        switchStateClientRpc(state, id);
    }
    [ClientRpc(RequireOwnership = false)]
    void switchStateClientRpc(int state, ulong id)
    {
        if (id == GameManager.Instance.ActualPlayer.GetComponent<NetworkObject>().NetworkObjectId) return;
        switchState(state);
    }

    public void Update()
    {
        if (IsServer)
        {
            currentEnergy.Value -= consumptionRate * currentState/(stateNumber-1) * Time.deltaTime;
            if(currentEnergy.Value <= 0)
            {
                currentEnergy.Value = 0;
                switchStateClientRpc(0, GameManager.Instance.ActualPlayer.GetComponent<NetworkObject>().NetworkObjectId);
                switchState(0);
            }

        }

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

    public void OnItemRemove()
    {
        switchState(0);
    }

    public void OnItemUnuse()
    {
        
    }
}
