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
    [SerializeField] Transform Player;
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
        Vector2 lookInput = lookAction.ReadValue<Vector2>();

        yRotation += lookInput.x * mouseSensitivity;
        xRotation -= lookInput.y * mouseSensitivity;

        xRotation = Mathf.Clamp(xRotation, -89f, 89f);

        //rotate camera
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //rotate player 
        Player.localRotation = Quaternion.Euler(0, yRotation, 0);
        //rotate orientation
        orientation.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
