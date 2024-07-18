using UnityEngine;
using Unity.Netcode;

public class PlayerActivationLogic : NetworkBehaviour
{ 
    [SerializeField] private Camera playerCamera;

    private void Start()
    {
        if(IsOwner)
        {
            playerCamera.gameObject.SetActive(true);
            InputManager.Instance.ActualPlayer = gameObject;
            InputManager.Instance.PlayerCamera = playerCamera;
        }
        else{
            playerCamera.gameObject.SetActive(false);
        }
    }

}

