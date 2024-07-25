using Unity.Netcode;
using UnityEngine;

public class NetworkFixedTransformEmitter : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Vector3 receivedPosition;
    Quaternion receivedRotation;

    private void FixedUpdate()
    {
        if (IsServer)
        {
            SendTransformInfoClientRpc(transform.position, transform.rotation);
        }
        else if(IsClient)
        {
            transform.position = receivedPosition;
            transform.rotation = receivedRotation;
        }
        
    }

    [ClientRpc(RequireOwnership = false)]
    public void SendTransformInfoClientRpc(Vector3 position, Quaternion rotation)
    {
        receivedPosition = position;
        receivedRotation = rotation;
    }

}
