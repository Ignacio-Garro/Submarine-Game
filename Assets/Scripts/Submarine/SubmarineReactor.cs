
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class SubmarineReactor : NetworkBehaviour
{
    [SerializeField] Transform CentreFuelColumnDownPosition;
    [SerializeField] Transform CentreFuelColumnUpPosition;
    [SerializeField] float rodClimbSpeed = 1; 
    List<FuelRod> centralRodList = new List<FuelRod>();

    public void InsertNewFuelRod(GameObject fuelRod)
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


    public void ExtractFuelRod(FuelRod fuelRod)
    {
        if (!IsServer) return;
        centralRodList.Remove(fuelRod);
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
                if(item.transform.localPosition.y < objectiveHeigth)
                {
                    item.transform.localPosition = Vector3.MoveTowards(item.transform.localPosition, new Vector3(CentreFuelColumnUpPosition.localPosition.x, objectiveHeigth, CentreFuelColumnUpPosition.localPosition.z), rodClimbSpeed * Time.deltaTime);
                }
                objectiveHeigth -= item.ySize / 2;
            }
        }
    }

}
