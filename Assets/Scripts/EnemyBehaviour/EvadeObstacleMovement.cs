using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvadeObstacleMovement : NetworkBehaviour, IdleMovementInterface
{
    [SerializeField] float speed = 1;
    [SerializeField] float wallVisionDistance = 10;
    [SerializeField] Vector3 preferredDirection = Vector3.forward;
    [SerializeField] int damage = 35;
    [SerializeField] float farRange = 10;
    [SerializeField] float nearRange = 3;
    [SerializeField] float inflatedTime = 5f;
    [SerializeField] Animator animator;
    Transform target = null;
    bool contactDamage = false;
    Vector3 PreferredDirection => target == null ? preferredDirection : target.position - transform.position;
    float inflatedTimer = 0;
    float preferredDirectionWeigth = 1;
    List<PlayerHealth> posibleTargets = new List<PlayerHealth>();

    Vector3 movementDirection = Vector3.forward;
    bool isMoving = true;
    bool isInflated = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    public override void OnNetworkSpawn()
    {
        movementDirection = Vector3.forward * speed;
        posibleTargets = FindObjectsByType<PlayerHealth>(FindObjectsSortMode.None).ToList();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;

        if (isInflated)
        {
            inflatedTimer += Time.deltaTime;
            if(inflatedTimer > inflatedTime)
            {
                inflatedTimer = 0;
                if ((target.position - transform.position).magnitude > farRange)
                {
                    ProceduralAnimator animator = GetComponent<ProceduralAnimator>();
                    if (animator != null)
                    {
                        animator.Demorph(() => { contactDamage = true; isInflated = false; target = null; });
                    }
                    this.animator.SetBool("inflated", false);
                    isInflated = false; 
                    preferredDirectionWeigth = 1;
                }
                
            }
            movementDirection *= (1 - Time.deltaTime);
            transform.Rotate(new Vector3(10, 10, 10) * Time.deltaTime);
            transform.position += movementDirection * speed * Time.deltaTime;
            return;
        }

        if (isMoving)
        {
            transform.position += movementDirection * speed * Time.deltaTime;
            transform.LookAt(transform.position + movementDirection);
            CalculateTurns();
        }
        if(target == null)
        {
            Transform nearest = null;

            foreach(PlayerHealth target in posibleTargets)
            {
                if((target.transform.position - transform.position).magnitude < farRange)
                {
                    if(nearest == null || (nearest.transform.position - transform.position).magnitude > (target.transform.position - transform.position).magnitude)
                    {
                        nearest = target.transform;
                    }
                }
            }
            if (nearest != null) DetectPlayerFar(nearest);
        }
        if(target != null)
        {
            Transform nearest = target;
            foreach (PlayerHealth target in posibleTargets)
            {
                if ((target.transform.position - transform.position).magnitude < farRange)
                {
                    if ((nearest.transform.position - transform.position).magnitude > (target.transform.position - transform.position).magnitude)
                    {
                        nearest = target.transform;
                    }
                }
            }
            if (nearest == null) target = null;
            else
            {
                if (nearest != target) DetectPlayerFar(nearest);
                if ((target.transform.position - transform.position).magnitude < nearRange) DetectPlayerClose(target);
            }

        }
    }

    void CalculateTurns()
    {

        Vector3 force = Vector3.zero;
        List<Vector3> directions = new List<Vector3> {Vector3.forward, Vector3.back, Vector3.up, Vector3.down, Vector3.right, Vector3.left, (Vector3.forward + Vector3.right).normalized, (Vector3.forward + Vector3.left).normalized, (Vector3.forward + Vector3.up).normalized, (Vector3.forward + Vector3.down).normalized, (Vector3.back + Vector3.up).normalized, (Vector3.back + Vector3.down).normalized, (Vector3.back + Vector3.right).normalized, (Vector3.back + Vector3.left).normalized, (Vector3.up + Vector3.left).normalized, (Vector3.up + Vector3.right).normalized, (Vector3.down + Vector3.left).normalized, (Vector3.down + Vector3.right).normalized };
        
        foreach(Vector3 direction in directions)
        {
            Debug.DrawRay(transform.position, direction * wallVisionDistance, Color.red);
            if (Physics.Raycast(transform.position, direction, out RaycastHit hit, wallVisionDistance) && hit.collider.GetComponent<PlayerHealth>() == null)
            {
                force += direction * hit.distance / wallVisionDistance;
            }
            else
            {
                force += direction;
            }
        }
        force += PreferredDirection * preferredDirectionWeigth;
        movementDirection += force * Time.deltaTime;        
        if(movementDirection.magnitude > 1) movementDirection.Normalize();

    }

    void TouchPlayer(PlayerHealth health)
    {
        if (contactDamage)
        {
            health.TakeDamage(damage);
        }
    }

    void DetectPlayerClose(Transform player)
    {
        ProceduralAnimator animator = GetComponent<ProceduralAnimator>();
        if (animator != null)
        {
            animator.Morph(() => { contactDamage = true; });
        }
        isInflated = true;
        this.animator.SetBool("inflated", true);
    }

    void DetectPlayerFar(Transform player)
    {
        target = player;
        preferredDirectionWeigth = 3;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            TouchPlayer(collision.gameObject.GetComponent<PlayerHealth>());
        }
    }



    Vector3 DecodeDirection(Vector3 direction)
    {
        if (direction == transform.right) return Vector3.right;
        if (direction == -transform.right) return Vector3.left;
        if (direction == transform.up) return Vector3.up;
        if (direction == -transform.up)  return Vector3.down;
        return Vector3.zero;
    }

    Vector3 EncodeDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return transform.right;
        if (direction == Vector3.left) return -transform.right;
        if (direction == Vector3.up) return transform.up;
        if (direction == Vector3.down) return -transform.up;
        return Vector3.zero;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void StartMovement()
    {
        throw new System.NotImplementedException();
    }

    public void StopMovement()
    {
        throw new System.NotImplementedException();
    }
}
