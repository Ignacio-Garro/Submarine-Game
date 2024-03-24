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
    [SerializeField] private TextMeshProUGUI textBox;
    [SerializeField] private TextMeshProUGUI textUI;
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

        if(engineState == EngineState.Destroyed || fuel <= 0){
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
        textBox.text = "Fuel: " + fuel + "\nPressure: " + pressure + "\nState: " + engineState + "\nengineRunning: " + engineRunning;
        textUI.text = "Fuel: " + fuel + "\nPressure: " + pressure + "\nState: " + engineState + "\nengineRunning: " + engineRunning;
    }

    public void RefillEnginefuel(int amountrefueled){
        fuel = fuel + amountrefueled;
        if(fuel > 0){
            submarineMovement.SetworkingEngine(true);
        }
    }

    public void ChangeEngineStatue(bool engineRunning){
        if(engineRunning == true){
            if(engineState != EngineState.Destroyed && fuel > 0){
                this.engineRunning = engineRunning;
            }
        }
        else{
            this.engineRunning = engineRunning;
        }
    }
}
