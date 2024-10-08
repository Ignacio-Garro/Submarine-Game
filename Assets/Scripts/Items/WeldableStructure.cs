using System;
using Unity.Netcode;
using UnityEngine;

public class WeldableStructure : NetworkBehaviour
{

    [SerializeField] Transform barPosition;
    [SerializeField] Canvas worldCanvas;
    [SerializeField] int requiredWelding = 3;
    NetworkVariable<float> weldingPercent = new NetworkVariable<float>();
    bool isBroken = false;
    int currentWeldPower = 0;
    Bar progressBar = null;

    public override void OnNetworkSpawn()
    {
        GameObject barObject = Instantiate(GameManager.Instance.FloatingBarPrefab, barPosition);
        progressBar = barObject.GetComponent<Bar>();
        progressBar?.gameObject.SetActive(false);
        progressBar?.transform.SetParent(worldCanvas.transform);
        progressBar.transform.position = barPosition.position;
    }

    public void StartWelding(int weldPower)
    {
        StartWeldingServerRpc(weldPower);
    }

    public void StopWelding(int weldPower)
    {
        StopWeldingServerRpc(weldPower);
    }

    [ServerRpc(RequireOwnership =false)]
    public void StartWeldingServerRpc(int wieldPower)
    {
        currentWeldPower += wieldPower;
        Debug.Log("Added: " + currentWeldPower);
    }
    [ServerRpc(RequireOwnership = false)]
    public void StopWeldingServerRpc(int wieldPower)
    {
        currentWeldPower -= wieldPower;
        Debug.Log("Released: " + currentWeldPower);
    }


    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            if (isBroken)
            {
                weldingPercent.Value += Time.deltaTime * currentWeldPower;
                if(weldingPercent.Value >= requiredWelding)
                {
                    isBroken = false;
                    weldingPercent.Value = 0;
                    Weld();
                    WeldClientRpc();
                }
            }
        }
        if (IsClient){
            if(weldingPercent.Value > 0 && weldingPercent.Value < requiredWelding)
            {
                progressBar.gameObject.SetActive(true);
            }
            progressBar.SetBarPercentage(weldingPercent.Value * 100f / requiredWelding);
        }
    }

    public virtual void Weld() { }

    [ClientRpc(RequireOwnership = false)]
    public void WeldClientRpc()
    {
        progressBar.gameObject.SetActive(false);
    }

    public void Break()
    {
        isBroken = true;
    }

}
