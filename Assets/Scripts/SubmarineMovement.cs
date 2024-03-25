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
    [SerializeField] float rotateVelocity = 25;
    [SerializeField] float acceptRange = 0.1f;
    [SerializeField] bool workingEngine;

    [Header("nav info")]
    [SerializeField] bool isMovingForward;
    [SerializeField] bool isMovingBackWards;
    [SerializeField] bool isMovingRight;
    [SerializeField] bool isMovingLeft;

    [Header("buttons")]
    [SerializeField] private ForwardButton forwardButton;
    [SerializeField] private BackwardsButton backwardsButton;
    [SerializeField] List<Transform> pathPoints;
    

    

    float velocity;
    Transform targetPoint;
    Rigidbody rigidBody;
    
    // Start is called before the first frame update
    private void Awake()
    {
        
    }

    void Start()
    {
        GetNextPoint();
        Rigidbody rigidBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
       
        if (isMovingForward)
        {
            velocity = Mathf.Min(velocity + forwardsAcceleration * Time.deltaTime, maxVelocity);
        }
        else
        {
            velocity = Mathf.Max(velocity - backWardsAcceleration * Time.deltaTime, 0);
        }
        if(targetPoint == null) { return; }
        if((gameObject.transform.position - targetPoint.position).magnitude < acceptRange)
        {
            GetNextPoint();
        }
        

    }

    void FixedUpdate()
    {
        transform.position += transform.forward * velocity * Time.fixedDeltaTime;
        if (targetPoint == null)
        {
            return;
        }
        if (isMovingForward)
        {
            Vector3 newDirection = Vector3.RotateTowards(gameObject.transform.forward, targetPoint.position-gameObject.transform.position, rotateVelocity*2*Mathf.PI/360 * Time.fixedDeltaTime , 0);
            gameObject.transform.rotation = Quaternion.LookRotation(newDirection);
        }
        
    }

    void GetNextPoint()
    { 
        if (pathPoints.Any())
        {
            targetPoint = pathPoints[0];
            pathPoints.RemoveAt(0);
        }
        else
        {
            targetPoint = null;
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
