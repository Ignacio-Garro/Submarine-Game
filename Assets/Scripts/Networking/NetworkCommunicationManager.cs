using Unity.Netcode;
using UnityEngine;


public class NetworkCommunicationManager : NetworkBehaviour
{

    public static NetworkCommunicationManager Instance;
    private void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }
    public void DestroyNetworkObject(NetworkObject networkObjectToDestroy)
    {
        networkObjectToDestroy.Despawn();
        Destroy(networkObjectToDestroy.gameObject);
    }

    
    public GameObject SpawnNetworkObject(GameObject prefabToSpawn, Vector3 spawnPosition)
    {
        GameObject spawnedObject = Instantiate(prefabToSpawn, position: spawnPosition, new Quaternion());
        NetworkObject networkObject = spawnedObject.GetComponent<NetworkObject>();
        networkObject.Spawn();
        return spawnedObject;
    }


    //-----------------------Server Rpcs-------------------------------


    [ServerRpc(RequireOwnership = false)]
    public void DestroyNetworkObjectServerRpc(NetworkObjectReference objectToDestroy)
    {
        objectToDestroy.TryGet(out NetworkObject networkObject);
        DestroyNetworkObject(networkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnNetworkObjectServerRpc(itemType item, Vector3 spawnPosition)
    {
        InventoryInfoManager.Instance.SpawnableObjects.TryGetValue(item, out GameObject prefabToSpawn);
        if (prefabToSpawn == null) return;
        SpawnNetworkObject(prefabToSpawn, spawnPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SpawnNetworkWithForceObjectServerRpc(itemType item, Vector3 spawnPosition, Vector3 forwardDirection, float throwForce)
    {
        InventoryInfoManager.Instance.SpawnableObjects.TryGetValue(item, out GameObject prefabToSpawn);
        if (prefabToSpawn == null) return;
        GameObject thrownItem = SpawnNetworkObject(prefabToSpawn, spawnPosition);
        Rigidbody rb = thrownItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(forwardDirection * throwForce, ForceMode.VelocityChange);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateItemServerRpc(NetworkObjectReference player, itemType item)
    {
        ActivateItemClientRpc(player, item);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateItemServerRpc(NetworkObjectReference player, itemType item)
    {
        DeactivateItemClientRpc(player, item);
    }

    //-------------------------Client_Rpcs-----------------------------

    [ClientRpc(RequireOwnership = false)]
    public void ActivateItemClientRpc(NetworkObjectReference player, itemType item)
    {
        player.TryGet(out NetworkObject playerNetworkObject);
        if (playerNetworkObject == null) return;
        //This prevents calling the sender
        if (playerNetworkObject.IsOwner) return;
        PlayerInventory currentPlayerInventory = playerNetworkObject.GetComponent<PlayerInventory>();
        if (currentPlayerInventory != null)
        {
            currentPlayerInventory.ShowItem(item);
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void DeactivateItemClientRpc(NetworkObjectReference player, itemType item)
    {
        player.TryGet(out NetworkObject playerNetworkObject);
        if (playerNetworkObject == null) return;
        //This prevents calling the sender
        if (playerNetworkObject.IsOwner) return;
        PlayerInventory currentPlayerInventory = playerNetworkObject.GetComponent<PlayerInventory>();
        if (currentPlayerInventory != null)
        {
            currentPlayerInventory.HideItem(item);
        }
    }

}
