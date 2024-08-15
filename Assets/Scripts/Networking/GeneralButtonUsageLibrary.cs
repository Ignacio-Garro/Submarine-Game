using UnityEngine;

public class GeneralButtonUsageLibrary : MonoBehaviour
{
    
    public static void ExtractItemFromPlayer(GameObject player, ItemPickable item)
    {
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.ExtractItemForcefully(item);
        }
    }
    
}
