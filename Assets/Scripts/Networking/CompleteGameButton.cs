using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class CompleteGameButton : NetworkBehaviour, IInteractuableObject
{
    
    public UnityEvent<GameObject> onClickLocal = null;
    public UnityEvent<GameObject> onClickServer = null;
    public UnityEvent<GameObject> onClickClients = null;

    public void OnInteract(GameObject playerThatInteracted)
    {
        onClickLocal.Invoke(playerThatInteracted);
        ClickServerRpc(playerThatInteracted);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ClickServerRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        onClickServer.Invoke(networkObject == null ? null : networkObject.gameObject);
        ClickClientRpc(playerThatInteracted);
    }

    [ClientRpc(RequireOwnership = false)]
    public void ClickClientRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        onClickClients.Invoke(networkObject == null ? null : networkObject.gameObject);
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
