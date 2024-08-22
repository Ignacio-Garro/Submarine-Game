
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SubmarineReactor : NetworkBehaviour
{
    [SerializeField] Transform CentreFuelColumnDownPosition;
    [SerializeField] Transform CentreFuelColumnUpPosition;
    [SerializeField] float rodClimbSpeed = 1;
    [SerializeField] bool isGodMode = false;
    [SerializeField] float averageCalculateTime = 1;
    [SerializeField] float lowOverheatLimit = 100000;
    [SerializeField] float highOverheatLimit = 200000;
    List<FuelRod> centralRodList = new List<FuelRod>(); 
    float energyUsedPerSecond = 0;
    float energyUsedPerSecondAverage = 0;
    
    float secondTimer = 0;

     
    public void InsertNewFuelRod(GameObject player, ItemPickable fuelRod)
    {

        if (!IsServer) return;
        
        Collider collider = fuelRod.GetComponent<Collider>();
        if (collider != null) collider.enabled = false;
        FuelRod rod = fuelRod.GetComponent<FuelRod>();
        if (rod == null) return;
        float targetHeight = CentreFuelColumnDownPosition.localPosition.y + rod.ySize/2;
        if (centralRodList.Any())
        {
            if (centralRodList.Last().transform.localPosition.y - centralRodList.Last().ySize < targetHeight) return;
        }
        fuelRod.GetComponent<Rigidbody>().isKinematic = true;
        fuelRod.transform.localPosition = new Vector3(CentreFuelColumnDownPosition.localPosition.x, targetHeight, CentreFuelColumnDownPosition.localPosition.z);
        fuelRod.transform.localRotation = CentreFuelColumnDownPosition.localRotation;
        centralRodList.Add(rod);
        rod.currentReactor = this;
    }

    public float TryToExctractEnergy(float energyAmmount)
    {

        if (!centralRodList.Any() || !centralRodList.First().IsInPlace) return 0;
        float availableEnergy = centralRodList.First().TryToExtractEnergy(energyAmmount);
        energyUsedPerSecond += availableEnergy;
        if (isGodMode) return energyAmmount;
        else return availableEnergy;
    }

    public void ExtractFuelRod(FuelRod fuelRod)
    {
        if (!IsServer) return;
        centralRodList.Remove(fuelRod);
        foreach (var item in centralRodList)
        {
            item.IsInPlace = false;
        }
    }

    private void Update()
    {
        if (!IsServer) return;
        if (centralRodList.Any())
        {
            float objectiveHeigth = CentreFuelColumnUpPosition.localPosition.y;
            foreach (FuelRod item in centralRodList)
            {
                objectiveHeigth -= item.ySize/2;
                if(item.transform.localPosition.y < objectiveHeigth && !item.IsInPlace)
                {
                    item.transform.localPosition = Vector3.MoveTowards(item.transform.localPosition, new Vector3(CentreFuelColumnUpPosition.localPosition.x, objectiveHeigth, CentreFuelColumnUpPosition.localPosition.z), rodClimbSpeed * Time.deltaTime);
                }
                else if(!item.IsInPlace) 
                {
                    item.IsInPlace = true;
                }
                objectiveHeigth -= item.ySize / 2;
            }
        }
        secondTimer += Time.deltaTime;
        if(secondTimer > averageCalculateTime)
        {
            secondTimer -= averageCalculateTime;
            energyUsedPerSecondAverage = (energyUsedPerSecondAverage * (60 - averageCalculateTime) + energyUsedPerSecond * averageCalculateTime) / 60;
            energyUsedPerSecond = 0;
        }
        if(energyUsedPerSecondAverage > lowOverheatLimit)
        {

        }
    }

}
