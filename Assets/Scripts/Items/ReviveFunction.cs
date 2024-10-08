using Unity.Netcode;
using UnityEngine;

public class ReviveFunction : BurstEnergyItemFunction
{
    [SerializeField] int reviveHealth = 10;

    public override void Burst(GameObject interactingObject)
    {
        interactingObject?.GetComponent<PlayerHealth>()?.PlayerRevives(reviveHealth);
    }

    [ServerRpc(RequireOwnership = false)]
    public void RevivePlayerServerRpc(NetworkObjectReference player)
    {
        RevivePlayerClientRpc(player);
    }

    [ClientRpc(RequireOwnership = false)]
    public void RevivePlayerClientRpc(NetworkObjectReference player)
    {
        player.TryGet(out NetworkObject playerObject);
        playerObject.GetComponent<PlayerHealth>()?.PlayerRevives(reviveHealth);
    }

}
