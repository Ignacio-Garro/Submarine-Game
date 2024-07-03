using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickable : MonoBehaviour, IPickable
{
    public ItemSO ItemScriprableObject;
    
    public void PickItem()
    {
        Destroy(gameObject);
    }
}

