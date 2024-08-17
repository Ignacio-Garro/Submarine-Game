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
    bool isActive = false;
    Vector2 lookInput => InputManager.Instance.LookInputBlock;

    private NetworkVariable<float> leverPosition = new NetworkVariable<float>(0);
    private float localLeverPosition = 0;
    private bool lastUsage = false;
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
                leverPosition.Value = Mathf.Clamp(leverPosition.Value + mouseMovement, -100f, 100f);
            }
            else
            {
                localLeverPosition = Mathf.Clamp(localLeverPosition + mouseMovement, -100f, 100f);
                ChangeleverPositionServerRpc(localLeverPosition);
            }
        }
        LeverMovable.position = leverTransformPosition;
        Debug.Log(LeverPosition);
    }

    public void ReleaseLastUsage()
    {
        if (!isActive) lastUsage = false;
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeleverPositionServerRpc(float leverValue)
    {
        leverPosition.Value = leverValue;
    }



}
