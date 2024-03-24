using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private bool Alive;
    [SerializeField] private int oxigenLevels;
    [SerializeField] private int breathingAmountOxigen;
    [SerializeField] private TextMeshProUGUI textPlayerStats;
    void Start()
    {
        InvokeRepeating("oxigenBreathing", 0f, 1f);
    }
    

    // Update is called once per frame
    void Update()
    {
        UpdateText();

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
    private void UpdateText(){
        textPlayerStats.text = "Alive: " + Alive + "\nOxigenLevels: " + oxigenLevels;
    }
}
