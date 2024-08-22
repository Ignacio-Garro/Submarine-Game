using UnityEngine;

public class SteamManager : MonoBehaviour
{

    public uint appId;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        try
        {
            Steamworks.SteamClient.Init(appId, true);
        }
        catch(System.Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void OnApplicationQuit()
    {
        Steamworks.SteamClient.Shutdown();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
