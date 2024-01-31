using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 3f;
    [SerializeField] float movementSpeed = 3f;
    [SerializeField] Transform cameraTransform;

    CharacterController controller;
    Vector2 look;

    private void Awake() {
        controller = GetComponent<CharacterController>();
    }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        UpdateLook();
        UpdateMovements();
    }


    private void UpdateLook() {
        look.x += Input.GetAxis("Mouse X") * mouseSensitivity;
        look.y += Input.GetAxis("Mouse Y") * mouseSensitivity;

        look.y = Mathf.Clamp(look.y, -89f, 89f);

        //rotate camer for y look
        cameraTransform.localRotation = Quaternion.Euler(-look.y, 0, 0);
        //rotate player with x look
        transform.localRotation = Quaternion.Euler(0, look.x, 0);
    }
    private void UpdateMovements() {
        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");

        var input = new Vector3();

        input += transform.right * x;
        input += transform.forward * y;
        //diagonals dont go over 1
        input = Vector3.ClampMagnitude(input, 1f);

        //transform.Translate(input * movementSpeed * Time.deltaTime, Space.World);
        controller.Move(input * movementSpeed * Time.deltaTime);
    }
}
