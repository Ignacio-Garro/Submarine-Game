using Unity.Netcode;
using UnityEngine;

public class ChargeStation : NetworkBehaviour
{
    [SerializeField] SubmarineController controller;
    [SerializeField] Transform itemPosition;
    [SerializeField] float ChargeRate = 100000;
    [SerializeField] ScreenBar screen;
    EnergyItemFunction currentItem;
    SubmarineReactor reactor => controller.reactor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        screen.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentItem == null) return;
        float energy = reactor.TryToExctractEnergy(Mathf.Min(ChargeRate * Time.deltaTime, currentItem.GetRemainingMaxCharge()));
        currentItem.Charge(energy);
        if(screen != null) screen.SetBarPercentage(currentItem.GetRemainingEnergyPercentage());
    }

    //Comprueba en server si puede meter el item
    public void TryToInsertItem(GameObject player, ItemPickable fuelRod)
    {
        if (!IsServer) return;
        EnergyItemFunction energy = fuelRod.GetComponent<EnergyItemFunction>();
        if (energy == null) return;
        if (currentItem != null) return;
        InsertNewItemClientRpc(player, fuelRod.gameObject);
    }

    //Jugador actualiza info si se puede meter el item y suelta el item
    [ClientRpc(RequireOwnership = false)]
    public void InsertNewItemClientRpc(NetworkObjectReference playerThatInteracted, NetworkObjectReference item)
    {
        playerThatInteracted.TryGet(out NetworkObject player);
        if (player == null) return;
        item.TryGet(out NetworkObject energyObj);
        if (energyObj == null) return;
        ItemPickable energyItem = energyObj.GetComponent<ItemPickable>();
        if (energyItem == null) return;
        screen.gameObject.SetActive(true);
        if (GameManager.Instance.ActualPlayer != player.gameObject) return;
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.ExtractItemForcefully(energyItem);
        }
        InsertNewItemServerRpc(player, item);
    }

    //Se actualiza info en el server, esto debe de ir despues del cliente porque el cliente tiene que soltar el item previamente
    [ServerRpc(RequireOwnership = false)]
    public void InsertNewItemServerRpc(NetworkObjectReference playerThatInteracted, NetworkObjectReference item)
    {

        if (!IsServer) return;
        playerThatInteracted.TryGet(out NetworkObject player);
        if (player == null) return;
        item.TryGet(out NetworkObject energyObj);
        if (energyObj == null) return;
        ItemPickable energyItem = energyObj.GetComponent<ItemPickable>();
        if (energyItem == null) return;
        Collider collider = energyItem.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        EnergyItemFunction energy = energyItem.GetComponent<EnergyItemFunction>();
        energyItem.GetComponent<Rigidbody>().isKinematic = true;
        energyItem.transform.position = itemPosition.position;
        energyItem.transform.rotation = itemPosition.rotation;
        currentItem = energy;
        energy.currentChargeStation = this;
    }



    public void ExtractCurrentItem()
    {
        currentItem = null;
        ExtractCurrentItemClientRpc();
    }

    [ClientRpc(RequireOwnership = false)]
    public void ExtractCurrentItemClientRpc()
    {
        screen.gameObject.SetActive(false);
    }


}
