using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkGameButton : NetworkBehaviour, IInteractuableObject
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public UnityEvent<GameObject> onClick = null;

    public void OnInteract(GameObject playerThatInteracted)
    {
        ClickServerRpc(playerThatInteracted);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClickServerRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        onClick.Invoke(networkObject == null ? null : networkObject.gameObject);
    }

    public void OnEnterInRange()
    {
        InputManager.Instance.AddInteractuableMaterial(gameObject);
    }

    public void OnExitInRange()
    {
        InputManager.Instance.RemoveInteractuableMaterial(gameObject);
    }
}
