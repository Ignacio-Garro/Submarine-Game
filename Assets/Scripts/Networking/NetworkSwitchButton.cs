using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkSwitchButton : NetworkBehaviour, IInteractuableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UnityEvent<GameObject> onClickOn = null;
    public UnityEvent<GameObject> onClickOff = null;
    bool pressed = false;
    public void OnInteract(GameObject playerThatInteracted)
    {
        SwitchServerRpc(playerThatInteracted);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchServerRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        GameObject playerObject = networkObject == null ? null : networkObject.gameObject;
        if (pressed)
        {
            onClickOff.Invoke(playerObject);
        }
        else
        {
            onClickOn.Invoke(playerObject);
        }
        pressed = !pressed;
    }

}
