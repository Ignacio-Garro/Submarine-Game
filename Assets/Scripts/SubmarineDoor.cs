using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineDoor : MonoBehaviour, IInteractuableObject
{

    [SerializeField] private Transform InsidePosition;
    [SerializeField] private Transform Submarine;
    public void OnInteract(MonoBehaviour playerThatInteracted)
    {
        playerThatInteracted.gameObject.transform.position = InsidePosition.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
