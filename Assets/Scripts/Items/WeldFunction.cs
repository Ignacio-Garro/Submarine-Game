using UnityEngine;

public class WeldFunction : MonoBehaviour, ItemFunctionInterface
{
    [SerializeField] private int weldPower = 1;
    public int WeldPower => weldPower;
    bool inUse = false;

    public void OnItemInView(GameObject interactingObject)
    {
        if (!inUse) return;
        interactingObject?.GetComponent<WeldableStructure>()?.StartWelding(weldPower);
    }

    public void OnItemOutOfView(GameObject interactingObject)
    {
        if (!inUse) return;
        interactingObject?.GetComponent<WeldableStructure>()?.StopWelding(weldPower);
    }

    public void OnItemRemove(GameObject interactingObject)
    {
        if (!inUse) return;
        interactingObject?.GetComponent<WeldableStructure>()?.StopWelding(weldPower);
        inUse = false;
    }

    public void OnItemUnuse(GameObject interactingObject)
    {
        if (!inUse) return;
        interactingObject?.GetComponent<WeldableStructure>()?.StopWelding(weldPower);
        inUse = false;
    }

    public void OnItemUse(GameObject interactingObject)
    {
        if (inUse) return;
        interactingObject?.GetComponent<WeldableStructure>()?.StartWelding(weldPower);
        inUse = true;
    }
}
