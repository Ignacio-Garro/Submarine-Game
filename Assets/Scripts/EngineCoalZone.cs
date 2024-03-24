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
            onTriggerEnter.Invoke();
            // coal is no more with us
            other.gameObject.SetActive(false);
            inputController.dropObject();
        }
    }
}