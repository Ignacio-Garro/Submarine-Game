using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private bool Alive;
    [SerializeField] private int oxigenLevels;
    [SerializeField] private int breathingAmountOxigen;
    [SerializeField] private IGrabbableObject grabbedIObject;
    [SerializeField] private GameObject grabbedGameObject;
    void Start()
    {
        InvokeRepeating("oxigenBreathing", 0f, 1f);
    }
    

    // Update is called once per frame
    void Update()
    {
        if(oxigenLevels <= 0){
            Alive = false;
        }
        
    }
    private void oxigenBreathing(){
        if(Alive){
            oxigenLevels = oxigenLevels - breathingAmountOxigen;
            if(oxigenLevels < 0) oxigenLevels = 0;
        }
    }

    public void setGrabbedObject(IGrabbableObject grabbedObject){
        this.grabbedIObject = grabbedObject;
        GetGrabbedGameObject();
    }
    public IGrabbableObject getGrabbedIObject(){
        return grabbedIObject;
    }
    public GameObject getGrabbedObject(){
        return grabbedGameObject;
    }

    public GameObject GetGrabbedGameObject()
    {
        if (grabbedIObject != null && grabbedIObject is MonoBehaviour)
        {
            MonoBehaviour monoBehaviour = (MonoBehaviour)grabbedIObject;
            grabbedGameObject = monoBehaviour.gameObject;
            return grabbedGameObject;
        }
        else
        {
            return null;
        }
    }
}
