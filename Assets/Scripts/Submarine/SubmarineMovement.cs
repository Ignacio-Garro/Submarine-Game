using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("sub nav controls")]
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float forwardsAcceleration = 1;
    [SerializeField] float backWardsAcceleration = 2;
    [SerializeField] float Deceleration = 1;
    [SerializeField] float rotateVelocity = 25;
    [SerializeField] bool workingEngine;

    [Header("nav info")]
    [SerializeField] bool isMovingForward;
    [SerializeField] bool isMovingBackWards;
    [SerializeField] bool isMovingRight;
    [SerializeField] bool isMovingLeft;

    [Header("buttons")]
    [SerializeField] private ForwardButton forwardButton;
    [SerializeField] private BackwardsButton backwardsButton;
    
    float velocity;
    Transform targetPoint;
    private Vector3 rotation;

    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        //Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
        rotation = new Vector3();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (isMovingForward && !isMovingBackWards)
        {
            velocity = Mathf.Min(velocity + forwardsAcceleration * Time.deltaTime, maxVelocity);
        }
        else if (!isMovingForward && isMovingBackWards)
        {
            velocity = Mathf.Max(velocity - backWardsAcceleration * Time.deltaTime, -maxVelocity);
        }
        else{
            velocity = Mathf.MoveTowards(velocity, 0, Deceleration * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        transform.position += transform.forward * velocity * Time.fixedDeltaTime;

        // Reset rotation to zero every frame to accumulate rotation only when keys are pressed
        Vector3 rotation = Vector3.zero;

        if (isMovingRight)
        {
            rotation += Vector3.up * rotateVelocity * Time.fixedDeltaTime;
        }
        else if (isMovingLeft)
        {
            rotation -= Vector3.up * rotateVelocity * Time.fixedDeltaTime;
        }

        // Apply rotation to the transform
        transform.Rotate(rotation);
    }


    

    public void SetForwardMovement(bool forwardmovement)
    {
        isMovingForward = forwardmovement;
    }
    public void SetBackWardsMovement(bool backwardsMovement)
    {
        isMovingBackWards = backwardsMovement;
    }
    public void SetRightMovement(bool rightMovement)
    {
        isMovingRight = rightMovement;
    }
    public void SetLeftMovement(bool leftMovement)
    {
        isMovingLeft = leftMovement;
    }

    public void SetworkingEngine(bool engineState){
        if(!engineState){
            isMovingForward = false;
            isMovingBackWards = false;
            isMovingRight = false;
            isMovingLeft = false;

            forwardButton.setPressed(false);
            backwardsButton.setPressed(false);
        }
        workingEngine = engineState;
    }

}
