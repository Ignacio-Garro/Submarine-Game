using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class PlayerMovementAdvanced : MonoBehaviour {

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction crouchAction;
    InputAction sprintAction;


    [Header("State")]
    [SerializeField] public MovementState state;

    [Header("Movement")]
    [SerializeField] private float currentSpeed;
    [SerializeField] private float currentDrag;
    [SerializeField] private float swimSpeed;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float groundDrag;
    [SerializeField] private float waterDrag;
    [SerializeField] private float stillDrag;
    [SerializeField] private bool isSprinting;

    [Header("Jumping")]
    [SerializeField] private bool isJumping;
    [SerializeField] private float airDrag;
    [SerializeField] private float jumpForce;
    [SerializeField] private float swimUpForce;
    [SerializeField] private float swimDownForce;
    [SerializeField] private float extraSpaceToJump;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private bool isCrouching;
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;

    [Header("Terrain Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private bool grounded;
    [SerializeField] private bool inWater;
    [SerializeField] private LayerMask whatIsGround;


    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;
    private bool exitingSlope;

    public Transform orientation;

    Vector3 moveDirection;

    Rigidbody rb;

    static Vector2 moveInput;

    public enum MovementState {
        still,
        walking,
        sprinting,
        crouching,
        air,
        swimming
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;


        //inputsystem
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];
        crouchAction = playerInput.actions["crouch"];
        sprintAction = playerInput.actions["sprint"];

        jumpAction.started += OnJumpStarted;
        jumpAction.canceled += OnJumpCanceled;

        crouchAction.started += OnCrouchStarted;
        crouchAction.canceled += OnCrouchCanceled;

        sprintAction.started += OnSprintStarted;
        sprintAction.canceled += OnSprintCanceled;
    }

    private void Update() {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + extraSpaceToJump, whatIsGround);
        MyInput();
        StateHandler();

        // handle drag
        if(inWater){
            rb.drag = waterDrag;
            currentDrag = waterDrag;
        }
        else if (state == MovementState.still){
            rb.drag = stillDrag;
            currentDrag = stillDrag;
        }
        else if (grounded){
            rb.drag = groundDrag;
            currentDrag = groundDrag;
        }
        else{
            rb.drag = airDrag;
            currentDrag = airDrag;
        }
    }

    private void FixedUpdate() { // fisicas
        MovePlayer();
        SpeedControl();
    }

    private void MyInput() {
        //horizontalInput = Input.GetAxisRaw("Horizontal");
        //verticalInput = Input.GetAxisRaw("Vertical");
        moveInput = moveAction.ReadValue<Vector2>();

        //var moveIn

        // In water SWIMMING MOVEMENT
        if(inWater){
            if (isJumping) {
                rb.AddForce(transform.up * swimUpForce);
            }
            if (isCrouching) {
                rb.AddForce(-transform.up * swimDownForce);
            }
        }
        //in ground
        else{
            // when to jump
            if (isJumping && readyToJump && grounded && !inWater) {
                readyToJump = false;

                Jump();

                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private void StateHandler() {
        //Mode - Swimming
        if(inWater){
            state = MovementState.swimming;
            currentSpeed = swimSpeed;
        }
        else{
            //Still
            if(grounded && moveInput.x == 0 && moveInput.y == 0){
                state = MovementState.still;
            }
            // Mode - Crouching
            else if (isCrouching) {
                state = MovementState.crouching;
                currentSpeed = crouchSpeed;
            }

            // Mode - Sprinting
            else if (isSprinting) {
                state = MovementState.sprinting;
                currentSpeed = sprintSpeed;
            }

            // Mode - Walking
            else if (grounded) {
                state = MovementState.walking;
                currentSpeed = walkSpeed;
            }

            // Mode - Air
            else if(!grounded){
                state = MovementState.air;
            }
        }
    }

    private void MovePlayer() {

        //IN GROUND
        if(!inWater){
            // calculate movement direction
            moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

            // on slope
            if (OnSlope() && !exitingSlope) {
                rb.AddForce(GetSlopeMoveDirection() * currentSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

            // on ground
            else if (grounded)
                rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);

            // in air
            else if (!grounded)
                rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);

            // turn gravity off while on slope
            rb.useGravity = !OnSlope();
        }

        //IN WATER
        else{
            // calculate movement direction
            moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

            rb.AddForce(moveDirection.normalized * currentSpeed * 5f, ForceMode.Force);
        }
    }

    private void SpeedControl() {

        // limiting speed on slope
        if (OnSlope() && !exitingSlope) {
            if (rb.velocity.magnitude > currentSpeed)
                rb.velocity = rb.velocity.normalized * currentSpeed;
        }

        // limiting speed on ground or in air or water
        else {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > currentSpeed) {
                Vector3 limitedVel = flatVel.normalized * currentSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump() {       
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump() {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    public void IsInWater(bool water){
        inWater = water;
    }


    private void OnJumpStarted(InputAction.CallbackContext context) {
        isJumping = true;
    }

    private void OnJumpCanceled(InputAction.CallbackContext context) {
        isJumping = false;
    }

    private void OnCrouchStarted(InputAction.CallbackContext context) {
        isCrouching = true;
    }

    private void OnCrouchCanceled(InputAction.CallbackContext context){
        transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        isCrouching = false;
    }

    private void OnSprintStarted(InputAction.CallbackContext context){
        isSprinting = true;
    }

    private void OnSprintCanceled(InputAction.CallbackContext context){
        isSprinting = false;
    }


    private void OnJump(){
        // when to jump
        if (readyToJump && grounded && !inWater) {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void OnCrouch(){
        transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
    }

}