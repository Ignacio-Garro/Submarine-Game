using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Engine : MonoBehaviour
{

    [Header("Engine info")]
    [SerializeField] private int fuel;
    [SerializeField] private int pressure;
    [SerializeField] private int pressureIncrease;
    [SerializeField] private int fuelSpending;
    [SerializeField] private bool engineRunning;
    [SerializeField] private EngineState engineState;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private SubmarineMovement submarineMovement;

    public enum EngineState {
        Normal,
        Emergency,
        Destroyed
    }

    private void Start(){
        InvokeRepeating("EngineHandeling", 0f, 1f);
    }

    private void Update(){

        UpdateText();

        if(fuel <= 0 || engineState == EngineState.Destroyed){
            engineRunning = false;
            submarineMovement.SetworkingEngine(false);
        }

        //pressure handeling
        if(pressure < 100){
            engineState = EngineState.Normal;
        }
        else if(pressure < 150 && pressure >= 100 && engineState != EngineState.Destroyed){
            engineState = EngineState.Emergency;
        }
        else if(pressure >= 150){
            engineState = EngineState.Destroyed;
        }
    }

    void EngineHandeling()
    {
        //engine working
        if(engineRunning && fuel > 0){
            fuel = fuel - fuelSpending;
            pressure = pressure + pressureIncrease;
        }
        //Engine not working
        else{
            if(pressure > 0 ){
                pressure--;
            }
        }
    }


    private void UpdateText(){
        text.text = "Fuel: " + fuel + "\nPressure: " + pressure + "\nState: " + engineState ;
    }

    public void RefillEnginefuel(int amountrefueled){
        fuel = fuel + amountrefueled;
    }

    public void ChangeEngineStatue(bool engineState){
        this.engineRunning = engineState;
    }
}
