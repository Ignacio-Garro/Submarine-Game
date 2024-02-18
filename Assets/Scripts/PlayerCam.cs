using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    InputAction lookAction;
    InputAction moveAction;

    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private float mouseSensitivity;
    [SerializeField] Transform orientation;
    float xRotation;
    float yRotation;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //input system
        lookAction = playerInput.actions["look"];
        moveAction  = playerInput.actions["move"];
    }
    void Update()
    {
        UpdateLook();
    }

    private void UpdateLook() {
        //float mouseX = Input.GetAxisRaw("Mouse X") * mouseSensitivity * Time.deltaTime;
        //float mouseY = Input.GetAxisRaw("Mouse Y") * mouseSensitivity * Time.deltaTime;

        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        yRotation += lookInput.x * mouseSensitivity;
        xRotation -= lookInput.y * mouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        //rotate player with x look
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0);
        //rotate camer for y look
        orientation.localRotation = Quaternion.Euler(0, yRotation, 0);
    }
}
