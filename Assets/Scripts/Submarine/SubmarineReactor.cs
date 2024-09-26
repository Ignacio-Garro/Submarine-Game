
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class SubmarineReactor : NetworkBehaviour
{
    [SerializeField] Transform CentreFuelColumnDownPosition;
    [SerializeField] Transform CentreFuelColumnUpPosition;
    [SerializeField] float rodClimbSpeed = 1;
    [SerializeField] bool isGodMode = false;
    [SerializeField] float averageCalculateTime = 1;
    [SerializeField] float lowOverheatLimit = 100000;
    [SerializeField] float highOverheatLimit = 200000;
    [SerializeField] float overheatTime = 10;
    [SerializeField] ScreenBar pressureBar;
    FuelRod upRod;
    FuelRod downRod;
    float energyUsedPerSecond = 0;
    bool rodIsClimbing = false;
    NetworkVariable<float> energyUsedPerSecondAverage = new NetworkVariable<float>(0f);
    
    bool isWorking = true;
    ReaparableStructure reparableComponent;
    RefrigerableStructure refrigerComponent;

    public override void OnNetworkSpawn()
    {
        reparableComponent = GetComponent<ReaparableStructure>();
        if(reparableComponent != null)
        {
            reparableComponent.repairClient += () => isWorking = true;
            reparableComponent.repairServer += () => pressureBar.Fix();
        }
        refrigerComponent = GetComponent<RefrigerableStructure>();
        if (refrigerComponent != null)
        {
            refrigerComponent.RefrigerateServerAction += (ammount) => { energyUsedPerSecondAverage.Value = Mathf.Max(energyUsedPerSecondAverage.Value - highOverheatLimit * ammount, 0); };
        }
    }


    public void InsertNewFuelRod(GameObject player, ItemPickable fuelRod)
    {

        if (!IsServer) return;
        
        Collider collider = fuelRod.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        FuelRod rod = fuelRod.GetComponent<FuelRod>();
        if (rod == null) return;
        float targetHeight = CentreFuelColumnDownPosition.localPosition.y + rod.ySize/2;
        if (downRod != null) return;
        
        fuelRod.GetComponent<Rigidbody>().isKinematic = true;
        fuelRod.transform.localPosition = new Vector3(CentreFuelColumnDownPosition.localPosition.x, targetHeight, CentreFuelColumnDownPosition.localPosition.z);
        fuelRod.transform.localRotation = CentreFuelColumnDownPosition.localRotation;
        downRod = rod;
        rod.currentReactor = this;
    }

    public float TryToExctractEnergy(float energyAmmount)
    {
        if (isGodMode)
        {
            energyUsedPerSecond += energyAmmount;
            return energyAmmount;
        }
        if (!isWorking) return 0;
        if (upRod == null) return 0;
        float availableEnergy = upRod.TryToExtractEnergy(energyAmmount);
        energyUsedPerSecond += availableEnergy;
        
        return availableEnergy;
    }

    public void ExtractFuelRod(FuelRod fuelRod)
    {
        if (!IsServer) return;
        if (fuelRod == downRod) downRod = null;
        else if(fuelRod == upRod) upRod = null;
    }

    public void PushRodUp()
    {
        if (upRod != null) return;
        if (downRod == null) return;
        rodIsClimbing = true;
    }

    private void Update()
    {
        pressureBar.SetBarPercentage(energyUsedPerSecondAverage.Value * 100 / highOverheatLimit);
        if (!IsServer) return;
        if (rodIsClimbing)
        {
            downRod.transform.localPosition = Vector3.MoveTowards(downRod.transform.localPosition, CentreFuelColumnUpPosition.localPosition, rodClimbSpeed * Time.deltaTime);
            if (downRod.transform.position == CentreFuelColumnUpPosition.position)
            {
                rodIsClimbing = false;
                upRod = downRod;
                downRod = null;
            }
            
        }
        
        energyUsedPerSecondAverage.Value += ((energyUsedPerSecond / Time.deltaTime) - energyUsedPerSecondAverage.Value) * Time.deltaTime / overheatTime;
        energyUsedPerSecond = 0;
        if (energyUsedPerSecondAverage.Value >= highOverheatLimit && isWorking)
        {
            reparableComponent.Break();
            isWorking = false;
            pressureBar.Break();
        }
    }

    public void RepairServer()
    {
        energyUsedPerSecondAverage.Value = 0;
        isWorking = true;
    }

    public void RepairClient()
    {
        isWorking = true;
        pressureBar.Fix();
    }
}
