using Unity.Netcode;
using UnityEngine;

public class RotationLever : NetworkBehaviour
{
    [SerializeField] Transform leverObject;
    [SerializeField] float sensitivity = 0.1f;
    [SerializeField] float rotationSensitivity = 0.1f;
    [SerializeField] float maxRotationSpeed = 5f;
    [SerializeField] float screenLimit = 50f;
    [SerializeField] float correctionForce = 1f;
    bool isActive = false;
    Vector2 posInScreen = Vector2.zero;
    float currentRotation = 0f;
    


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

            leverObject.eulerAngles = new Vector3(leverObject.eulerAngles.x, leverObject.eulerAngles.y, currentRotation);
            posInScreen = Vector3.MoveTowards(posInScreen, Vector3.zero, Time.deltaTime * correctionForce);
        }
    }

}
