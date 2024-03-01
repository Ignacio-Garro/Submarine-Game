using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ladder : MonoBehaviour
{
    [SerializeField] private string tagFilter;
    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private UnityEvent onTriggerExit;

    private void OnTriggerEnter(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && other.gameObject.CompareTag(tagFilter)){
            onTriggerEnter.Invoke();
            Debug.Log("enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!String.IsNullOrEmpty(tagFilter) && other.gameObject.CompareTag(tagFilter)){
            onTriggerExit.Invoke();
            Debug.Log("exit");
        }
    }
}
