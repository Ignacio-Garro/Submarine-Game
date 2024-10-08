using UnityEngine;
using Unity.Netcode;

public class PlayerActivationLogic : NetworkBehaviour
{ 
    [SerializeField] private Camera playerCamera;
    [SerializeField] GameObject body;
    [SerializeField] GameObject arms;

    private void Start()
    {
        if(IsOwner)
        {
            playerCamera.gameObject.SetActive(true);
            GameManager.Instance.ActualPlayer = gameObject;
            GameManager.Instance.PlayerCamera = playerCamera;
            body.SetActive(false);
            arms.SetActive(true);
        }
        else{
            playerCamera.gameObject.SetActive(false);
        }
    }

}

