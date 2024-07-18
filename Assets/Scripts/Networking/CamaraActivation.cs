using UnityEngine;
using Unity.Netcode;

public class CamaraActivation : NetworkBehaviour
{ 
    [SerializeField] private GameObject playerCamera;

    private void Start()
    {
        if(IsOwner)
        {
            playerCamera.SetActive(true);
        }
        else{
            playerCamera.SetActive(false);
        }
    }

}

