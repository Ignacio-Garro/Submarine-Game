using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCam : MonoBehaviour
{
    Vector2 lookInput => InputManager.Instance.LookInputNormal;

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
    }
    void Update()
    {
        UpdateLook();
        MoveCamera();
    }

    private void UpdateLook() {
        yRotation += lookInput.x * mouseSensitivity;
        xRotation -= lookInput.y * mouseSensitivity;
        xRotation = Mathf.Clamp(xRotation, -89f, 89f);
    }

    private void MoveCamera()
    {
        //rotate camera
        transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        //rotate player 
        Player.localRotation = Quaternion.Euler(0, yRotation, 0);
        //rotate orientation
        orientation.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }
}
