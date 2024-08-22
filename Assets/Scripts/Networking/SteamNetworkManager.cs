using Netcode.Transports.Facepunch;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

public class SteamNetworkManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public SteamNetworkManager Instance = null;

    private FacepunchTransport transport = null;

    private Lobby? currentLobby;
    public Lobby? CurrentLobby => currentLobby;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (Instance != null)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        transport = GetComponent<FacepunchTransport>();
        SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
        SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
        SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
        SteamMatchmaking.OnLobbyMemberLeave += OnLobbyMemberLeave;
        SteamMatchmaking.OnLobbyInvite += OnLobbyInvite;
        SteamMatchmaking.OnLobbyGameCreated += OnLobbyGameCreated;
    }


    public void OnLobbyCreated(Result result, Lobby lobby)
    {

    }
    public void OnLobbyEntered(Lobby lobby)
    {

    }

    public void OnLobbyMemberJoined(Lobby lobby, Friend friend)
    {

    }

    public void OnLobbyMemberLeave(Lobby lobby, Friend friend)
    {

    }

    public void OnLobbyInvite(Friend friend, Lobby lobby)
    {

    }

    public void OnLobbyGameCreated(Lobby lobby, uint u, ushort s, SteamId id)
    {

    }

    public async void StartHostLobby(int maxPlayers = 4)
    {
        NetworkManager.Singleton.StartHost();
        currentLobby = await SteamMatchmaking.CreateLobbyAsync(maxPlayers);
    }

    public void Disconnect()
    {
        currentLobby?.Leave();
        NetworkManager.Singleton.Shutdown();
    }

    private void OnApplicationQuit() => Disconnect();
    
    public void StartClient(SteamId id)
    {
        if (NetworkManager.Singleton.StartClient())
        {
            transport.targetSteamId = id;
        }
    }

    
}
