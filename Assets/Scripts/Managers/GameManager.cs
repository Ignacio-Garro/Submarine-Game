using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static GameManager Instance;

    public GameObject FloatingBarPrefab;

    private Camera playerCamera;
    private GameObject actualPlayer;
    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value;}
    public GameObject ActualPlayer { get => actualPlayer; set => actualPlayer = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
}
