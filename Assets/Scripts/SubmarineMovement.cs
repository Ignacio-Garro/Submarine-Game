using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [Header("sub nav controls")]
    [SerializeField] float maxVelocity;
    [SerializeField] float forwardsAcceleration;
    [SerializeField] float backwardsAcceleration;
    [SerializeField] float rotateVelocity;
    [SerializeField] bool workingEngine;

    [Header("nav info")]
    [SerializeField] bool isMovingForward;
    [SerializeField] bool isMovingBackWards;
    [SerializeField] bool isMovingRight;
    [SerializeField] bool isMovingLeft;

    [Header("buttons")]
    [SerializeField] private ForwardButton forwardButton;
    [SerializeField] private BackwardsButton backwardsButton;

    // Start is called before the first frame update
    void Start()
    {
        //rigidBody = GetComponent<Rigidbody>();
        //rigidBody.maxLinearVelocity = maxVelocity;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        /*
        if (isMovingForward)
        {
            rigidBody.AddForce(gameObject.transform.forward * rigidBody.mass * forwardsAcceleration);
        }
        if (isMovingBackWards)
        {
            rigidBody.AddForce(gameObject.transform.forward * rigidBody.mass * -backwardsAcceleration);
        }
        */
        if (isMovingForward && workingEngine){
            transform.position += transform.forward * forwardsAcceleration * forwardsAcceleration * Time.deltaTime;
        }
        if (isMovingBackWards && workingEngine){
            transform.position += transform.forward * -backwardsAcceleration * backwardsAcceleration * Time.deltaTime;
        }

        if (isMovingRight && workingEngine)
        {
            Vector3 rotacionActual = transform.eulerAngles;
            rotacionActual.y -= rotateVelocity * Time.deltaTime;
            transform.eulerAngles = rotacionActual;
        }
        if (isMovingLeft && workingEngine)
        {
            Vector3 rotacionActual = transform.eulerAngles;
            rotacionActual.y += rotateVelocity * Time.deltaTime;
            transform.eulerAngles = rotacionActual;
        }
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
