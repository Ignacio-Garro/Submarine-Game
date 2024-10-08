using UnityEngine;

public class RepairFunction : MonoBehaviour, ItemFunctionInterface
{
    [SerializeField] private int repairPower = 1;
    public int RepairPower => repairPower;

    public void OnItemInView(GameObject interactingObject) { }
    public void OnItemOutOfView(GameObject interactingObject) { }
    public void OnItemRemove(GameObject interactingObject) { }
    public void OnItemUnuse(GameObject interactingObject) { }
    
    public void OnItemUse(GameObject interactingObject)
    {
        interactingObject?.GetComponent<ReaparableStructure>()?.TryToRepair(repairPower);
    }
}
