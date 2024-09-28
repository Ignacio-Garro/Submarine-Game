using UnityEngine;

public class RefrigerateFunction : MonoBehaviour
{
    [SerializeField] private int refrigerPower = 1;
    [SerializeField] private int maxUses = 20;
    public int RefrigerPower => refrigerPower;
    public int usesLeft = 0;

    private void Start()
    {
        usesLeft = maxUses;
    }

}
