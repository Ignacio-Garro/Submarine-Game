using UnityEngine;

public interface ItemInteractuableInterface
{
    public void OnInteract(ItemPickable item);
    public void OnStopInteracting(ItemPickable item);

    public void OnEnterInteractionRange(ItemPickable item);
    
}
