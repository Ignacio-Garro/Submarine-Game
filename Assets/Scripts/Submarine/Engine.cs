using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class Engine : ReaparableStructure
{

    [Header("Engine info")]
    [SerializeField] ScreenBar pressureBar;
    [SerializeField] float minTimeToExplode;
    [SerializeField] float maxTimeToExplode;
    [SerializeField] float timeToRegulatePressure = 50;
    [SerializeField] float explosionPoint = 70;
    
    bool isWorking = true;
    bool IsWorking => isWorking;

    private float probabilityPerSecond => pressureLevel.Value < explosionPoint ? 0 : 1 - Mathf.Pow(0.5f, 1f / timeToExplode);
    float timeToExplode => Mathf.Lerp(maxTimeToExplode, minTimeToExplode, (pressureLevel.Value - explosionPoint) / (100 - explosionPoint));

    NetworkVariable<float> pressureLevel = new NetworkVariable<float>(0f);

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsServer) InvokeRepeating("EngineHandeling", 0f, 1f);
    }

    private void Update(){
        pressureBar.SetBarPercentage(pressureLevel.Value);
    }

    void EngineHandeling()
    {
        if (isWorking && Random.Range(0f, 1f) <= probabilityPerSecond)
        {
            ExplodeClientRpc();
            Break();
        }
    }

    public float UseMotorPercentage(float percentage)
    {
        float absPercentage = Mathf.Abs(percentage);
        pressureLevel.Value += (absPercentage - pressureLevel.Value) * Time.deltaTime / timeToRegulatePressure;
        return isWorking ? percentage : 0f;
    }
    
    

    public override void RepairServer()
    {
        if (isWorking) return;
        pressureLevel.Value = 0;
        isWorking = true;
    }

    public override void RepairClient()
    {
        isWorking = true;
        pressureBar.Fix();
    }


    [ClientRpc(RequireOwnership = false)]
    void ExplodeClientRpc()
    {
        isWorking = false;
        pressureBar.Break();
    }
}
