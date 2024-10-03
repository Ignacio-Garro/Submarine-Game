using System;
using Unity.Netcode;
using UnityEngine;

public class Lever : NetworkBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private Transform startPosition;
    [SerializeField] private Transform endPosition;
    [SerializeField] private Transform LeverMovable;
    [SerializeField] float mouseSensitivity = 0.1f;
    [SerializeField] int numberOfDivisions = 5;
    bool isActive = false;
    Vector2 lookInput => InputManager.Instance.LookInputBlock;

    private NetworkVariable<float> leverPosition = new NetworkVariable<float>(0);
    private float localLeverPosition = 0;
    private bool lastUsage = false;
    private float continuousLeverPosition = 0;
    public float LeverPosition => Mathf.RoundToInt(leverPosition.Value);
    float freeLagLeverPosition => lastUsage && !IsServer ? localLeverPosition : leverPosition.Value;
    Vector3 leverTransformPosition => Vector3.Lerp(startPosition.position, endPosition.position, (freeLagLeverPosition + 100f) / 200f);
    

    public void InteractWithLever()
    {
        InputManager.Instance.InputIsBlocked = true;
        isActive = true;
        lastUsage = true;
        InputManager.Instance.onInteractReleasedAfterBlock += CancelInteract;
    }

    public void CancelInteract(GameObject player, Camera camera)
    {
        continuousLeverPosition = freeLagLeverPosition;
        isActive = false;
        InputManager.Instance.onInteractReleasedAfterBlock -= CancelInteract;
        InputManager.Instance.InputIsBlocked = false;
    }


    private void Update()
    {
        
        if (isActive)
        {
            float mouseMovement = lookInput.y * mouseSensitivity;
            if (IsServer)
            {
                continuousLeverPosition = Mathf.Clamp(continuousLeverPosition + mouseMovement, -100f, 100f);
                leverPosition.Value = ApproximateLeverToSegment();
            }
            else
            {
                continuousLeverPosition = Mathf.Clamp(continuousLeverPosition + mouseMovement, -100f, 100f);
                localLeverPosition = ApproximateLeverToSegment();
                ChangeleverPositionServerRpc(localLeverPosition);
            }
        }
        LeverMovable.position = leverTransformPosition;
       
    }

    public float ApproximateLeverToSegment()
    {
        int segmentPorcentage = 100 / (numberOfDivisions - 1);
        if ((int)continuousLeverPosition % segmentPorcentage > segmentPorcentage / 2) return (int)continuousLeverPosition + (segmentPorcentage - (int)continuousLeverPosition % segmentPorcentage);
        else return (int)continuousLeverPosition - (int)continuousLeverPosition % segmentPorcentage;
    }

    //Se llama en los clientes que no han usado la palanca cuando alguien la usa
    public void ReleaseLastUsage()
    {
        if (!isActive) lastUsage = false;
    }

    //Cambia el valor real de la palanca en el server
    [ServerRpc(RequireOwnership = false)]
    public void ChangeleverPositionServerRpc(float leverValue)
    {
        leverPosition.Value = leverValue;
    }



}
