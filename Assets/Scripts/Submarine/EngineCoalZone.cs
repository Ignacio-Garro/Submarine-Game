using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class EngineCoalZone : MonoBehaviour
{
    [SerializeField] private string tagFilter;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private InputController inputController;


    private void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && other.gameObject.CompareTag(tagFilter)){
            Coal coal = other.gameObject.GetComponent<Coal>(); // Get the Coal component

            if (coal != null){
                coal.AddFuelToEngineCoal(); // Call the function if Coal component exists
                onTriggerEnter.Invoke();
                other.gameObject.SetActive(false);
                //inputController.dropObject();
            }
        }
        else
        {
            Debug.LogError("Coal component not found on object with tag: " + tagFilter);
            return;
        }
    }
}