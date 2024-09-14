using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class PlayerMovement : NetworkBehaviour {

    PlayerInput playerInput;
    InputAction moveAction;
    InputAction lookAction;
    InputAction jumpAction;
    InputAction crouchAction;
    InputAction sprintAction;

    [Header("Network")]

    //everone can read but only server writes
    //private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1);
   


    [Header("State")]
    [SerializeField] public MovementState state;
    [SerializeField] private bool isSprinting;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isCrouching;
    [SerializeField] private bool inWater;
    [SerializeField] private bool inLadder;
    [SerializeField] private bool lookingAtLadder;
    [SerializeField] private bool isGrounded;

    [Header("Movement")]
    [SerializeField] private float currentSpeedXZ;
    [SerializeField] private float currentSpeedY;
    [SerializeField] private float targetSpeed;
    [SerializeField] private float currentDrag;
    [SerializeField] private float accelRate;
    [SerializeField] private float speedText;

    [Header("Speeds")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float swimSpeed;
    [SerializeField] private float ladderSpeed;
    [SerializeField] private float crouchSpeed;

    [Header("Drag")]
    [SerializeField] private float dragXZ;
    [SerializeField] private float dragY;
    [SerializeField] private float groundDrag;
    [SerializeField] private float waterDrag;
    [SerializeField] private float stillDrag;
    [SerializeField] private float airDrag;
    [SerializeField] private float ladderDrag;

    [Header("Swimming")]
    [SerializeField] private float swimUpForce;
    [SerializeField] private float swimDownForce;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private float fallingGravityMultiplier;
    [SerializeField] private float lowJumpGravityMultiplier;
    [SerializeField] private float extraSpaceToJump;
    
    private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchYScale;
    [SerializeField] private float startYScale;
    [SerializeField] private bool canUncrouch;
    [SerializeField] private float extraSpaceToUncrouch;
    private bool tryingToUncrouch;

    [Header("Terrain Check")]
    [SerializeField] private LayerMask whatIsGround;

    [Header("Player Info")]
    [SerializeField] private float playerHeight;
    [SerializeField] private Transform orientation;
    [SerializeField] private Transform objectGrabPointTransfrom;
    [SerializeField] private float interactionRange = 1f;
    [SerializeField] private Transform playerCamera;

    private Vector3 flatVel;

    public Transform ObjectGrabPointTransfrom
    {
        get{return objectGrabPointTransfrom;}
        set{objectGrabPointTransfrom = value;}
    }

    Vector3 moveDirection;

    Rigidbody rb;

    Vector2 moveInput => InputManager.Instance.MoveInputNormal;

    public enum MovementState {
        still,
        walking,
        sprinting,
        crouching,
        air,
        swimming,
        ladder
    }
    /*
    public override void OnNetworkSpawn(){
        randomNumber.OnValueChanged += (int previousValue, int NewValue) => {
            Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value);
        };
    }
    */
    private void Start() {
        rb = GetComponent<Rigidbody>();
        startYScale = transform.localScale.y;

        rb.freezeRotation = true;
        readyToJump = true;
        inLadder = false;
        inWater = false;
        tryingToUncrouch = false;


        //inputsystem
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        jumpAction = playerInput.actions["jump"];
        crouchAction = playerInput.actions["crouch"];
        sprintAction = playerInput.actions["sprint"];

        InputManager.Instance.onJumpPressed += (_,_) => OnJumpStarted();
        InputManager.Instance.onJumpReleased += (_, _) => OnJumpCanceled();
        InputManager.Instance.onCrouchPressed += (_, _) => OnCrouchStarted();
        InputManager.Instance.onCrouchReleased += (_, _) => OnCrouchCanceled();
        InputManager.Instance.onSprintPressed += (_, _) => OnSprintStarted();
        InputManager.Instance.onSprintReleased += (_, _) => OnSprintCanceled();

    }

    private void Update() {
        //Debug.Log(OwnerClientId + "; randomNumber: " + randomNumber.Value);
        if(!IsOwner) return;

        StateHandler();
        showDebugLines();
        DragControl();
        BetterJumping();
        SpeedLimitControl();
        LookatLadder();

        //current speeds
        currentSpeedXZ = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        currentSpeedY = rb.linearVelocity.y;

        speedText = currentSpeedXZ;

        // calculate movement direction
        if(!inWater){
            moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
            moveDirection.y = 0; // Set y-axis component to zero
        }
        else{
            moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        }
        
        // ground check
        //isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + extraSpaceToJump, whatIsGround);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.7f + extraSpaceToJump);

        //trying to uncrouch
        canUncrouch = !Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.5f + extraSpaceToUncrouch);

        if(tryingToUncrouch && canUncrouch && isCrouching){
            isCrouching = false;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void FixedUpdate() { // fisicas
        if(!IsOwner) return;
        MovePlayer();
        ManualDrag();
        SwimControlUpDown();
    }

   

    private void StateHandler() {
        // Mode - ladder
        if(inLadder){
            state = MovementState.ladder;
            targetSpeed = ladderSpeed;
        }
        // Mode - Swimming
        else if(inWater){
            state = MovementState.swimming;
            targetSpeed = swimSpeed;
        }
        else{
            // Mode - Still
            if(isGrounded && moveInput.x == 0 && moveInput.y == 0){
                state = MovementState.still;
            }
            // Mode - Crouching
            else if (isCrouching) {
                state = MovementState.crouching;
                targetSpeed = crouchSpeed;
            }

            // Mode - Sprinting
            else if (isSprinting) {
                state = MovementState.sprinting;
                targetSpeed = sprintSpeed;
            }

            // Mode - Walking
            else if (isGrounded) {
                state = MovementState.walking;
                targetSpeed = walkSpeed;
            }

            // Mode - Air
            else if(!isGrounded){
                state = MovementState.air;
            }
        }
    }

    private void MovePlayer() {
        //Debug.Log("moveDirection: " + moveDirection.z);
        //IN WATER
        if(inWater){
            rb.AddForce(moveDirection.normalized * targetSpeed * accelRate, ForceMode.Force);
        }

        //IN LADDER
        else if(inLadder && (!isGrounded || moveInput.y > 0)){

            if(lookingAtLadder){
                    moveDirection.x = 0f;
                    moveDirection.y = moveInput.y;
                    moveDirection.z = 0f;

                    if(moveInput.y != 0){
                        rb.linearVelocity = moveDirection.normalized * targetSpeed;
                    }
                    else{
                        rb.linearVelocity = Vector3.zero;
                    }
            }
            else{
                    if(moveInput.y != 0 || moveInput.x != 0){
                        rb.AddForce(moveDirection.normalized * targetSpeed * accelRate, ForceMode.Force);
                    }
                    else{
                        rb.linearVelocity = Vector3.zero;
                    }
            }
        }
        //IN GROUND
        else{
            // on ground
            if (isGrounded){
                rb.AddForce(moveDirection.normalized * targetSpeed * accelRate, ForceMode.Force);
            }

            // in air
            else if (!isGrounded)
                rb.AddForce(moveDirection.normalized * targetSpeed * accelRate * airMultiplier, ForceMode.Force);
        }
    }

    private void LookatLadder(){
        Debug.DrawRay(playerCamera.transform.position + Vector3.down * 0.8f, playerCamera.transform.forward * interactionRange, Color.red);
        if (Physics.Raycast(playerCamera.transform.position + Vector3.down * 0.8f, playerCamera.transform.forward, out RaycastHit hit, interactionRange)){
            if (hit.collider.CompareTag("Ladder")) {
                lookingAtLadder = true;
            }
            else {
                lookingAtLadder = false;
            }
        }
        else{
            lookingAtLadder = false;
        }
    }
    private void SwimControlUpDown(){
        //in water
        if(inWater){
            if (isJumping) {
                rb.AddForce(transform.up * swimUpForce * accelRate, ForceMode.Force);
            }
            if (isCrouching) {
                rb.AddForce(-transform.up * swimDownForce * accelRate, ForceMode.Force);
            }
        }
    }
    private void Jump() {       
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump() {
        readyToJump = true;
    }
    private void BetterJumping(){
        if(state == MovementState.air){
            if(rb.linearVelocity.y < 0f) {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (fallingGravityMultiplier - 1) * Time.deltaTime;
            }
            else if( rb.linearVelocity.y > 0f && !isJumping) {
                rb.linearVelocity += Vector3.up * Physics.gravity.y * (lowJumpGravityMultiplier - 1) * Time.deltaTime;
            }
        }
    }

    private void SpeedLimitControl() {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > targetSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * targetSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        
    }

    private void DragControl(){
        if(inWater){
            dragXZ = waterDrag;
            currentDrag = waterDrag;
        }
        else if (inLadder){
            dragXZ = ladderDrag;
            currentDrag = ladderDrag;
        }
        else if (state == MovementState.still){
            dragXZ = stillDrag;
            currentDrag = stillDrag;
        }
        else if (isGrounded){
            dragXZ = groundDrag;
            currentDrag = groundDrag;
        }
        else{
            dragXZ = airDrag;
            currentDrag = airDrag;
        }
    }

    private void ManualDrag(){
        if(state == MovementState.swimming){
            rb.linearDamping = waterDrag;
        }
        else{
            // Apply drag separately in each axis
            Vector3 drag = new Vector3(rb.linearVelocity.x * -dragXZ, rb.linearVelocity.y * -dragY, rb.linearVelocity.z * -dragXZ);
            rb.AddForce(drag, ForceMode.Force);

            rb.linearDamping = 0f;
        }

    }

    private void showDebugLines(){
        //drawDebugLine(moveDirection,Color.magenta);
        //drawDebugLine(orientation.forward,Color.green);
        //drawDebugLine(rb.velocity,Color.red);
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

    //INPUT-------------------------------------------------------

    private void OnJumpStarted() {
        isJumping = true;
        // when to jump
        if (isJumping && isGrounded && readyToJump) {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void OnJumpCanceled() {
        isJumping = false;
    }

    private void OnCrouchStarted() {
        if(!inWater || isGrounded){
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }
        isCrouching = true;
        tryingToUncrouch = false;
    }

    private void OnCrouchCanceled(){
        tryingToUncrouch = true;
    }

    private void OnSprintStarted(){
        isSprinting = true;
    }

    private void OnSprintCanceled(){
        isSprinting = false;
    }
}