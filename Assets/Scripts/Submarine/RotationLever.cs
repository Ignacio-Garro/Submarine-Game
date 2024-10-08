using Unity.Netcode;
using UnityEngine;

public class RotationLever : LeverParent
{
    [SerializeField] Transform leverObject;
    [SerializeField] float sensitivity = 0.1f;
    [SerializeField] float maxRotationSpeed = 5f;
    [SerializeField] float screenLimit = 50f;
    [SerializeField] float correctionForce = 1f;
    [SerializeField] float maxRotation = 720;
    [SerializeField] float divisions = 5;
    [SerializeField] float directionArrowMaxRotation = 70f;
    [SerializeField] Transform directionArrow;

    Vector3 arrowRotationOffset = Vector3.zero;
    Vector3 leverRotationOffset = Vector3.zero;

    bool isActive = false;
    Vector2 posInScreen = Vector2.zero;
    float currentRotation = 0;
    
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        arrowRotationOffset = directionArrow.localEulerAngles;
        leverRotationOffset = leverObject.localEulerAngles;
    }

    public void ChangeLeverState(bool state)
    {
        isActive = state;
        posInScreen = Vector2.zero;
    }

    private void Update()
    {
        if (isActive)
        {

            Vector2 previous = posInScreen;
            posInScreen += InputManager.Instance.LookInputBlock * sensitivity;
            posInScreen.x = Mathf.Clamp(posInScreen.x, -screenLimit, screenLimit);
            posInScreen.y = Mathf.Clamp(posInScreen.y, -screenLimit, screenLimit);
            Vector2 movement = posInScreen - previous;
            if(movement.magnitude > maxRotationSpeed * Time.deltaTime)
            {
                movement = movement.normalized * maxRotationSpeed * Time.deltaTime;
            }
            if (previous.y > 0)
            {
                currentRotation += movement.x;
            }
            else
            {
                currentRotation -= movement.x;
            }
            if (previous.x > 0)
            {
                currentRotation -= movement.y;
            }
            else
            {
                currentRotation += movement.y;
            }
            currentRotation = Mathf.Clamp(currentRotation, -maxRotation, maxRotation);
            UpdateCurrentRotationServerRpc(OwnerClientId, currentRotation);
            leverObject.localEulerAngles = new Vector3(0, 0, currentRotation) + leverRotationOffset;
            posInScreen = Vector3.MoveTowards(posInScreen, Vector3.zero, Time.deltaTime * correctionForce);
            UpdateProgressServerRpc((currentRotation + maxRotation) / (maxRotation * 2));
            directionArrow.localEulerAngles = new Vector3(0, 0, (Progress * 2 - 1)  * directionArrowMaxRotation) + arrowRotationOffset;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateCurrentRotationServerRpc(ulong id, float rotation)
    {
        UpdateCurrentRotationClientRpc(id, rotation);
    }

    [ClientRpc(RequireOwnership = false)]
    public void UpdateCurrentRotationClientRpc(ulong id, float rotation)
    {
        if (OwnerClientId == id) return;
        currentRotation = rotation;
    }


}
