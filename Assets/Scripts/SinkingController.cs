using Unity.Netcode;
using UnityEngine;

public class SinkingController : NetworkBehaviour
{

    [SerializeField] SubmarineController controller;
    [SerializeField] Transform initialWaterPosition;
    [SerializeField] Transform finalWaterPosition;
    [SerializeField] GameObject waterMassThatRises;
    [SerializeField] float sinkVelocityPerHole = 0.1f;
    [SerializeField] float sinkDistancePerPorcent = 1f;
    private float waterLevel = 0;
    public float WaterLevel => waterLevel;
    public float SinkDistancePerPorcent => sinkDistancePerPorcent;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        waterLevel += (controller.SinkingRate - controller.DrainRate) * sinkVelocityPerHole * Time.deltaTime;
        Mathf.Clamp(waterLevel, 0, 100);
        waterMassThatRises.transform.localPosition = Vector3.Lerp(initialWaterPosition.localPosition, finalWaterPosition.localPosition, waterLevel/100f);
    }


}
