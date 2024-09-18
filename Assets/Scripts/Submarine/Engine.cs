using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Engine : MonoBehaviour
{

    [Header("Engine info")]
    [SerializeField] ScreenBar pressureBar;
    [SerializeField] float minTimeToExplode;
    [SerializeField] float maxTimeToExplode;
    [SerializeField] float timeToRegulatePressure = 50;
    [SerializeField] float explosionPoint = 70;
    
    bool isWorking = true;
    bool IsWorking => isWorking;

    private float probabilityPerSecond => 1 - Mathf.Pow(0.5f, 1f / timeToExplode);
    float timeToExplode => Mathf.Lerp(maxTimeToExplode, minTimeToExplode, (pressureLevel - explosionPoint) / (100 - explosionPoint));

    float pressureLevel = 0f;


    private void Start(){
        InvokeRepeating("EngineHandeling", 0f, 1f);

    }

    private void Update(){
        pressureBar.SetBarPorcentage(pressureLevel);
    }

    void EngineHandeling()
    {
        if (isWorking && Random.Range(0f, 1f) <= probabilityPerSecond)
        {
            Explode();
        }
    }

    public float UseMotorPercentage(float percentage)
    {
        float absPercentage = Mathf.Abs(percentage);
        pressureLevel += (absPercentage - pressureLevel) * Time.deltaTime / timeToRegulatePressure;
        return isWorking ? percentage : 0f;
    }

    public void Fix()
    {
        isWorking = true;
        pressureBar.Fix();
    }

    void Explode()
    {
        isWorking = false;
        pressureBar.Break();
    }
}
