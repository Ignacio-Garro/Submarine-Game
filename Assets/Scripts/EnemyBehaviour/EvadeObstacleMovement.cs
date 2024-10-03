using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class EvadeObstacleMovement : NetworkBehaviour, IdleMovementInterface
{
    [SerializeField] float force = 3;
    [SerializeField] float speed = 3;
    [SerializeField] float wallVisionDistance = 10;
    [SerializeField] Vector3 preferredDirection = Vector3.forward;
    [SerializeField] int damage = 35;
    [SerializeField] float farRange = 10;
    [SerializeField] float nearRange = 3;
    [SerializeField] float inflatedTime = 5f;
    [SerializeField] float knockback = 15;
    [SerializeField] Animator animator;
    [SerializeField] Collider bigCollider;
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
        GetComponent<Rigidbody>().maxLinearVelocity = speed;
        bigCollider.enabled = false;
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
                        animator.Demorph(() => { contactDamage = true; isInflated = false; target = null; bigCollider.enabled = false; });
                    }
                    this.animator.SetBool("inflated", false);
                    isInflated = false; 
                    preferredDirectionWeigth = 1;
                }
                
            }
            
            return;
        }

        
        if(target == null)
        {
            Transform nearest = null;

            foreach(PlayerHealth target in posibleTargets)
            {
                if (target == null) continue;
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

    private void FixedUpdate()
    {
        if (!IsServer) return;
        if (isMoving && !isInflated)
        {
            CalculateTurns();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.rotation = Quaternion.LookRotation(rb.linearVelocity);
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
        force *= this.force;
        GetComponent<Rigidbody>()?.AddForce(force, ForceMode.Acceleration);
        
    }

    void TouchPlayer(PlayerHealth health)
    {
        if (contactDamage)
        {
            health.TakeDamage(damage);
            health.KnockBack(health.transform.position - transform.position, knockback);
        }
    }

    void DetectPlayerClose(Transform player)
    {
        ProceduralAnimator animator = GetComponent<ProceduralAnimator>();
        if (animator != null)
        {
            animator.Morph(() => { contactDamage = true; });
            bigCollider.enabled = true;
        }
        isInflated = true;
        this.animator.SetBool("inflated", true);
        GetComponent<Rigidbody>()?.AddTorque(Vector3.right, ForceMode.Impulse);
    }

    void DetectPlayerFar(Transform player)
    {
        target = player;
        preferredDirectionWeigth = 1f;
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerHealth>() != null)
        {
            TouchPlayer(collision.gameObject.GetComponent<PlayerHealth>());
        }
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
