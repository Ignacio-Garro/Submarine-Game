using Unity.Netcode;
using UnityEngine;

public class RotationLever : NetworkBehaviour
{
    [SerializeField] float sensitivuty = 0.1f;
    bool isActive = false;
    Vector2 posInScreen = Vector2.zero;
    public void ChangeLeverState(bool state)
    {
        isActive = state;
    }

    private void Update()
    {
        if (isActive)
        {
            posInScreen += InputManager.Instance.LookInputBlock * sensitivuty;
        }
    }

}
