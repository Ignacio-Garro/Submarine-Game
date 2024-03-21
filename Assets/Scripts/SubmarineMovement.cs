using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class SubmarineMovement : MonoBehaviour
{
    [SerializeField] float maxVelocity = 10;
    [SerializeField] float forwardsAcceleration = 10;
    [SerializeField] float backwardsAcceleration = 10;
    [SerializeField] float rotateVelocity = 25;
    [SerializeField] bool workingEngine;


    Rigidbody rigidBody;
    bool isMovingForward = false;
    bool isMovingBackWards = false;
    bool isMovingRight = false;
    bool isMovingLeft = false;

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
            transform.position += transform.forward * -backwardsAcceleration * -backwardsAcceleration * Time.deltaTime;
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
        workingEngine = engineState;
    }

}
