
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using static UnityEditor.Progress;


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


    //Reparents an object into the orientation object of the player. If orientation is not found does nothing
    [ServerRpc(RequireOwnership = false)]
    public void ReparentItemServerRpc(NetworkObjectReference childObject, NetworkObjectReference player)
    {
        childObject.TryGet(out NetworkObject childnetworkObject);
        player.TryGet(out NetworkObject playernetworkObject);
        if (childnetworkObject == null || playernetworkObject == null) return;
        Transform searched = null;
        Transform[] allChildren = playernetworkObject.GetComponentsInChildren<Transform>();
        foreach (Transform child in allChildren)
        {
            if (child.name == "ObjectGrabPoint")
            {
                searched = child;
                // Ahora tienes acceso al objeto "f"
                break;
            }
        }
        if(searched != null) childnetworkObject.transform.parent = searched;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReparentNetworkObjectServerRpc(NetworkObjectReference childObject, NetworkObjectReference parentObject)
    {
        childObject.TryGet(out NetworkObject childnetworkObject);
        parentObject.TryGet(out NetworkObject parentnetworkObject);
        if (childnetworkObject == null || parentnetworkObject == null) return;
        childnetworkObject.transform.parent = parentnetworkObject.transform;
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeattachFromParentNetworkObjectServerRpc(NetworkObjectReference childObject)
    {
        childObject.TryGet(out NetworkObject childnetworkObject);
        if (childnetworkObject == null) return;
        childnetworkObject.transform.parent = null;
    }



    [ServerRpc(RequireOwnership = false)]
    public void DestroyNetworkObjectServerRpc(NetworkObjectReference objectToDestroy)
    {
        objectToDestroy.TryGet(out NetworkObject networkObject);
        DestroyNetworkObject(networkObject);
    }

        

    [ServerRpc(RequireOwnership = false)]
    public void ActivateItemServerRpc(NetworkObjectReference player, NetworkObjectReference item)
    {
        ActivateItemClientRpc(player, item);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateItemServerRpc(NetworkObjectReference player, NetworkObjectReference item)
    {
        DeactivateItemClientRpc(player, item);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivatePhysicCollisionsServerRpc(NetworkObjectReference obj)
    {
        DeactivatePhysicCollisionsClientRpc(obj);
    }

    [ServerRpc(RequireOwnership =false)]
    public void ActivatePhysicCollisionsServerRpc(NetworkObjectReference obj)
    {
        ActivatePhysicCollisionsClientRpc(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateCollisionsServerRpc(NetworkObjectReference obj)
    {
        DeactivateCollisionsClientRpc(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateCollisionsServerRpc(NetworkObjectReference obj)
    {
        ActivateCollisionsClientRpc(obj);
    }


    [ServerRpc(RequireOwnership = false)]
    public void ChangeOwnerShipServerRpc(NetworkObjectReference obj, ulong newOwnerId)
    {
        obj.TryGet(out NetworkObject networkObject);
        if (networkObject == null) return;
        networkObject.ChangeOwnership(newOwnerId);
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateRigidBodyServerRpc(NetworkObjectReference obj)
    {
        DeactivateRigidBodyClientRpc(obj);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ActivateRigidBodyServerRpc(NetworkObjectReference obj)
    {
        ActivateRigidBodyClientRpc(obj);
    }


    [ServerRpc(RequireOwnership = false)]
    public void ActivateVisibilityServerRpc(NetworkObjectReference item)
    {

        ActivateVisibilityClientRpc(item);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DeactivateVisibilityServerRpc(NetworkObjectReference item)
    {

        DeactivateVisibilityClientRpc(item);
    }

    //-------------------------Client_Rpcs-----------------------------


    [ClientRpc(RequireOwnership = false)]
    public void DeactivateRigidBodyClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObject);
        Rigidbody rb = networkObject.gameObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    [ClientRpc(RequireOwnership = false)]
    public void ActivateRigidBodyClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObject);
        Rigidbody rb = networkObject.gameObject.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }


    [ClientRpc(RequireOwnership = false)]
    public void ActivatePhysicCollisionsClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObj);
        List<Transform> transformList = new List<Transform>();
        if (networkObj == null) return;
        Collider[] allChildren = networkObj.GetComponentsInChildren<Collider>(true);
        foreach (var item1 in allChildren)
        {
            item1.isTrigger = false;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void DeactivatePhysicCollisionsClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObj);
        List<Transform> transformList = new List<Transform>();
        if (networkObj == null) return;
        Collider[] allChildren = networkObj.GetComponentsInChildren<Collider>(true);
        foreach (var item1 in allChildren)
        {
            item1.isTrigger = true;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void ActivateCollisionsClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObj);
        List<Transform> transformList = new List<Transform>();
        if (networkObj == null) return;
        Collider[] allChildren = networkObj.GetComponentsInChildren<Collider>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = true;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void DeactivateCollisionsClientRpc(NetworkObjectReference obj)
    {
        obj.TryGet(out NetworkObject networkObj);
        List<Transform> transformList = new List<Transform>();
        if (networkObj == null) return;
        Collider[] allChildren = networkObj.GetComponentsInChildren<Collider>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = false;
        }

    }


    [ClientRpc(RequireOwnership = false)]
    public void ActivateVisibilityClientRpc(NetworkObjectReference item)
    {
        
        item.TryGet(out NetworkObject itemNetworkObject);
      
        if (itemNetworkObject == null) return;
        Renderer[] allChildren = itemNetworkObject.GetComponentsInChildren<Renderer>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = true;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void DeactivateVisibilityClientRpc(NetworkObjectReference item)
    {
      
        item.TryGet(out NetworkObject itemNetworkObject);
       
        if (itemNetworkObject == null) return;
        Renderer[] allChildren = itemNetworkObject.GetComponentsInChildren<Renderer>(true);
        foreach (var item1 in allChildren)
        {
            item1.enabled = false;
        }
    }

    [ClientRpc(RequireOwnership = false)]
    public void ActivateItemClientRpc(NetworkObjectReference player, NetworkObjectReference item)
    {
        player.TryGet(out NetworkObject playerNetworkObject);
        item.TryGet(out NetworkObject itemNetworkObject);
        if (playerNetworkObject == null) return;
        if(itemNetworkObject == null) return;
        //This prevents calling the sender
        if (playerNetworkObject.IsOwner) return;
        itemNetworkObject.gameObject.SetActive(true);
    }

    [ClientRpc(RequireOwnership = false)]
    public void DeactivateItemClientRpc(NetworkObjectReference player, NetworkObjectReference item)
    {
        player.TryGet(out NetworkObject playerNetworkObject);
        item.TryGet(out NetworkObject itemNetworkObject);
        if (playerNetworkObject == null) return;
        if (itemNetworkObject == null) return;
        //This prevents calling the sender
        if (playerNetworkObject.IsOwner) return;
        itemNetworkObject.gameObject.SetActive(false);
    }

}
