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
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private bool isSprinting;

    [Header("Drag")]
    [SerializeField] private float groundDrag;
    [SerializeField] private float waterDrag;
    [SerializeField] private float stillDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float ladderDrag;

    [Header("Swimming")]
    [SerializeField] private float swimSpeed;
    [SerializeField] private float swimUpForce;
    [SerializeField] private float swimDownForce;


    [Header("Ladders")]
    [SerializeField] private float ladderSpeed;

    [Header("Jumping")]
    [SerializeField] private bool isJumping;
    [SerializeField] private float jumpForce;
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
    [SerializeField] private bool inLadder;
    [SerializeField] private LayerMask whatIsGround;

    [Header("Player Info")]
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform objectGrabPointTransfrom;
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private Transform playerCamera;

    private Vector3 flatVel;

    public Transform ObjectGrabPointTransfrom
    {
        get{return objectGrabPointTransfrom;}
        set{objectGrabPointTransfrom = value;}
    }

    Vector3 moveDirection;

    Rigidbody rb;

    static Vector2 moveInput;

    public enum MovementState {
        still,
        walking,
        sprinting,
        crouching,
        air,
        swimming,
        ladder
    }

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        inLadder = false;
        inWater = false;
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

        // calculate movement direction
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        //LINEAS DEBUG EN SCENE
        drawDebugLine(moveDirection,Color.yellow);
        //drawDebugLine(orientation.forward,Color.green);
        //drawDebugLine(rb.velocity,Color.red);

        // handle drag
        if(inWater){
            rb.drag = waterDrag;
            currentDrag = waterDrag;
        }
        else if (inLadder){
            rb.drag = ladderDrag;
            currentDrag = ladderDrag;
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
        if(inLadder){
            state = MovementState.ladder;
            currentSpeed = ladderSpeed;
        }
        //Mode - Swimming
        else if(inWater){
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
    
        //IN WATER
        if(inWater){
            rb.AddForce(moveDirection.normalized * currentSpeed * 5f, ForceMode.Force);
        }

        //IN LADDER
        else if(inLadder && (!grounded || moveInput.y > 0)){
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange)){
                string targetTag = "Ladder";
                //LOOKING AT LADDER
                if (hit.collider.CompareTag(targetTag)){
                    // calculate movement direction
                    moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
                    
                    Debug.Log("yes");
                    moveDirection.y = moveDirection.x;
                    moveDirection.x = 0f;
                    drawDebugLine(moveDirection, Color.cyan);

                    rb.velocity = moveDirection.normalized * currentSpeed;
                }
                //ON LADDER BUT NO LOOKING AT LADDER
                else{
                    rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
                    Debug.Log("no");
                }
            }
            //ON LADDER BUT NO LOOKING AT ANYTHING
            else{
                    rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
                }
        }
         //IN GROUND
        else{
            moveDirection.y = 0; // Set y-axis component to zero

            // on ground
            if (grounded){
                rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Force);
                //rb.velocity = moveDirection.normalized * currentSpeed;
            }

            // in air
            else if (!grounded)
                rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Force);
                //rb.velocity = moveDirection.normalized * currentSpeed;
        }
    }

    private void SpeedControl() {

        flatVel.x = rb.velocity.x;
        flatVel.y = 0f;
        flatVel.z = rb.velocity.z;

        // limit velocity if needed
        if (flatVel.magnitude > currentSpeed) {
            Vector3 limitedVel = flatVel.normalized * currentSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        
    }

    private void Jump() {       
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump() {
        readyToJump = true;
    }


    private void drawDebugLine(Vector3 vector3, Color color) {
        // Define the origin point of the ray (start position)
        Vector3 origin = orientation.position;

        // Define the direction of the ray based on the orientation's forward direction
        Vector3 direction = vector3;

        // Length of the ray
        float rayLength = 10f; // Adjust this as needed

        // Draw the ray
        Debug.DrawRay(origin, direction * rayLength, color);
    }

    public void IsInWater(bool water){
        inWater = water;
    }
    public void IsInLadder(bool ladder){
        inLadder = ladder;
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