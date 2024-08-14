
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class FuelRod : NetworkBehaviour
{
    [SerializeField] float initialStoredEnergy = 50000;
    [SerializeField] Transform energyFluid;
    public SubmarineReactor currentReactor = null;
    public float ySize => GetYSize();

    float storedEnergy = 0;

    public void Start()
    {
        storedEnergy = initialStoredEnergy;
    }

    public float TryToExtractEnergy(float energy)
    {
        if (!IsServer) return 0;
        if (energy > storedEnergy) 
        {
            energy = storedEnergy;
        }
        storedEnergy -= energy;
        energyFluid.localScale =  new Vector3(1,storedEnergy / initialStoredEnergy,1);
        return energy;
    }
    
    public void RemoveFromReactor()
    {
        if(currentReactor != null) currentReactor.ExtractFuelRod(this);   
    }

    public float GetYSize()
    {
        if (this == null)
        {
            return 0;
        }
        float maxSize = 0;
        List<Transform> transforms = new List<Transform>();
        transforms.Add(transform);
        while(transforms.Any())
        {
            Transform currentObject = transforms[0];
            transforms.RemoveAt(0);
            foreach(Transform child in currentObject)
            {
                Renderer rend = child.GetComponent<Renderer>();
                if (rend != null)
                {
                    if (rend.bounds.size.y > maxSize) maxSize = rend.bounds.size.y;
                }
                transforms.Add(child);
            }
        }
        return maxSize;
    }

}
