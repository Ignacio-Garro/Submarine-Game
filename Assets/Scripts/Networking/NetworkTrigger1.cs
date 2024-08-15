
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class NetworkTrigger : NetworkBehaviour
{
    public UnityEvent<GameObject> onTriggerEnterEvent;
    public UnityEvent<GameObject> onTriggerExitEvent;
    //List of tags the object must have to trigger the trigger
    [SerializeField] List<string> acceptanceTags;
    [SerializeField] bool mustHaveAllTags = true;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colision:" + other.gameObject);
        if (!CheckOtherObject(other.gameObject)) return;
        EnterTriggerServerRpc(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!CheckOtherObject(other.gameObject)) return;
        ExitTriggerServerRpc(other.gameObject);
    }

    private bool CheckOtherObject(GameObject other)
    {
        if (mustHaveAllTags)
        {
            foreach (string tag in acceptanceTags)
            {
                if (!other.CompareTag(tag)) return false;
            }
        }
        else
        {
            bool found = false;
            foreach (string tag in acceptanceTags)
            {
                if (!found && other.CompareTag(tag)) found = true;
            }
            if (!found && acceptanceTags.Any()) return false;
        }
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    public void EnterTriggerServerRpc(NetworkObjectReference collideObject)
    {
        collideObject.TryGet(out NetworkObject networkObject);
        onTriggerEnterEvent.Invoke(networkObject == null ? null : networkObject.gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ExitTriggerServerRpc(NetworkObjectReference collideObject)
    {
        collideObject.TryGet(out NetworkObject networkObject);
        onTriggerExitEvent.Invoke(networkObject == null ? null : networkObject.gameObject);
    }
}
