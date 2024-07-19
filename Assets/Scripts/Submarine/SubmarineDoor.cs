using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineDoor : MonoBehaviour, IInteractuableObject
{

    [SerializeField] private Transform InsidePosition;
    [SerializeField] private GameObject targetParent;
    public void OnInteract(GameObject playerThatInteracted)
    {
        Rigidbody rb = playerThatInteracted.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.Sleep();
            rb.transform.position = InsidePosition.position;

             playerThatInteracted.transform.SetParent(targetParent.transform);
        }
    }
}
