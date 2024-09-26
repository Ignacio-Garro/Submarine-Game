using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ReaparableStructure : NetworkBehaviour, ItemInteractuableInterface
{
    [SerializeField] Transform barPosition;
    [SerializeField] Canvas worldCanvas;
    [SerializeField] int repairRequired = 1;
    public Action repairServer;
    public Action repairClient;

    NetworkVariable<int> currentRepair = new NetworkVariable<int>(0);
    Bar progressBar = null;
    NetworkVariable<bool> isBroken = new NetworkVariable<bool>(false);

    public override void OnNetworkSpawn()
    {
        GameObject barObject = Instantiate(GameManager.Instance.FloatingBarPrefab, barPosition);
        progressBar = barObject.GetComponent<Bar>();
        progressBar?.gameObject.SetActive(false);
        progressBar?.transform.SetParent(worldCanvas.transform);
        progressBar.transform.position = barPosition.position;
    }


    public void OnEnterInteractionRange(ItemPickable item)
    {
        
    }

    public void OnInteract(ItemPickable item)
    {
        RepairFunction repairItem = item.GetComponent<RepairFunction>();
        if (repairItem == null) return;
        if (!isBroken.Value) return;
        RepairServerRpc(repairItem.RepairPower);
    }

   

    [ServerRpc(RequireOwnership = false)]
    void RepairServerRpc(int repairPower)
    {
        currentRepair.Value += repairPower;
        if (currentRepair.Value >= repairRequired)
        {
            currentRepair.Value = 0;
            RepairServer();
            isBroken.Value = false;
        }
        RepairClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    void RepairClientRpc()
    {
        if (currentRepair.Value <= 0f)
        {
            progressBar?.gameObject.SetActive(false);
            RepairClient();
        }
        else
        {
            progressBar?.gameObject.SetActive(true);
            progressBar?.SetBarPercentage(100f * currentRepair.Value / repairRequired);
        }
    }

    public void RepairServer() {
        repairServer();
    }
    public void RepairClient() {
        repairClient();
    }
    public void Break()
    {
        isBroken.Value = true;
    }

    public void OnStopInteracting(ItemPickable item)
    {
        
    }
}
