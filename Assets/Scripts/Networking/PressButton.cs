using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class PressButton : MonoBehaviour, IInteractuableObject
{
    public UnityEvent<GameObject> onClickLocal = null;
    public UnityEvent<GameObject> onClickServer = null;
    public UnityEvent<GameObject> onClickClients = null;

    public void OnInteract(GameObject playerThatInteracted)
    {
        //InputManager.Instance.InputIsBlocked = true;
        //InputManager.Instance.onInteractReleasedAfterBlock += ReleaseButton;
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

/*
    public void ReleaseButton(GameObject player, Camera camera)
    {
        InputManager.Instance.InputIsBlocked = false;
        InputManager.Instance.onInteractReleasedAfterBlock -= ReleaseButton;
        onReleaseLocal.Invoke(player);
        ReleaseServerRpc(player);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReleaseServerRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        onReleaseServer.Invoke(networkObject == null ? null : networkObject.gameObject);
        ReleaseClientRpc(playerThatInteracted);
    }

    [ClientRpc(RequireOwnership = false)]
    public void ReleaseClientRpc(NetworkObjectReference playerThatInteracted)
    {
        playerThatInteracted.TryGet(out NetworkObject networkObject);
        onReleaseClients.Invoke(networkObject == null ? null : networkObject.gameObject);
    }
    */
}
