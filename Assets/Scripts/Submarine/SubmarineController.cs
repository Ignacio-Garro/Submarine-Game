using NUnit.Framework;
using Unity.Netcode;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{
    [SerializeField] private Transform EnterPosition;
    [SerializeField] private Engine submarineEngine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void EnterSubmarine(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.Sleep();
            rb.transform.position = EnterPosition.position;
            player.transform.parent = gameObject.transform;
        }
    }

    public void InsertCoal(GameObject coal)
    {
        submarineEngine.RefillEnginefuel(1);
        NetworkObject coalNetwork = coal.GetComponent<NetworkObject>();
        Assert.IsNotNull(coalNetwork);
        coalNetwork.Despawn();
        

    }
}
