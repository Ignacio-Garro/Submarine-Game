using System.Collections.Generic;
using UnityEngine;

public class InventoryInfoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public static InventoryInfoManager Instance;

    [Header("Item prefabs")]
    [SerializeField] GameObject RedCan_prefab;
    [SerializeField] GameObject BlueCan_prefab;

    private Dictionary<itemType, GameObject> spawnableObjects = new Dictionary<itemType, GameObject>() { };
    public Dictionary<itemType, GameObject> SpawnableObjects => spawnableObjects;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance);
        }
    }

    void Start()
    {
        spawnableObjects.Add(itemType.redCan, RedCan_prefab);
        spawnableObjects.Add(itemType.blueCan, BlueCan_prefab);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
