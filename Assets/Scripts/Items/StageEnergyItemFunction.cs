using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StageEnergyItemFunction : EnergyItemFunction
{
    [SerializeField] float consumptionRate = 1000f;
    protected int currentState = 0;
    protected int stateNumber = 2;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
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

    
    public override void OnItemUse(GameObject interactingObjects)
    {
        if (currentEnergy.Value <= 0) return;
        int nextState = (currentState + 1) % stateNumber;
        switchState(nextState);
        switchStateServerRpc(nextState, GameManager.Instance.ActualPlayer.GetComponent<NetworkObject>().NetworkObjectId);
    }

    public override void OnItemRemove(GameObject interactingObjects)
    {
        switchState(0);
    }

    public override void OnItemUnuse(GameObject interactingObjects) { }

    public override void OnItemOutOfView(GameObject interactingObjects) { }

    public override void OnItemInView(GameObject interactingObjects) { }

}
