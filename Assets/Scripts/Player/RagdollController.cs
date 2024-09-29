using UnityEngine;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Transform RagdollRoot;
    [SerializeField] private CapsuleCollider CapsuleCollider;
    [SerializeField] private bool StartRagdoll = false;
    [SerializeField] private PlayerMovement playerMovement;
    private bool isRagdollActive = false;
    // Only public for Ragdoll Runtime GUI for explosive force
    public Rigidbody[] Rigidbodies;
    private CharacterJoint[] Joints;
    private Collider[] Colliders;

    private void Awake()
    {
        Rigidbodies = RagdollRoot.GetComponentsInChildren<Rigidbody>();
        Joints = RagdollRoot.GetComponentsInChildren<CharacterJoint>();
        Colliders = RagdollRoot.GetComponentsInChildren<Collider>();

        playerMovement = GetComponentInParent<PlayerMovement>();

        if (StartRagdoll)
        {
            EnableRagdoll();
            isRagdollActive = true;
        }
        else
        {
            EnableAnimator();
            isRagdollActive = false;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (isRagdollActive)
            {
                EnableAnimator();
                playerMovement.ToggleAlive(true);//lives
            }
            else
            {
                EnableRagdoll();
                playerMovement.ToggleAlive(false);//dies
            }

            isRagdollActive = !isRagdollActive;  
        }
    }

    public void EnableRagdoll()
    {
        Animator.enabled = false;
        CapsuleCollider.enabled = false;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = true;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = true;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.detectCollisions = true;
            rigidbody.useGravity = true;
        }
    }

    public void EnableAnimator()
    {
        //minimize performace hit
        Animator.enabled = true;
        CapsuleCollider.enabled = true;
        foreach (CharacterJoint joint in Joints)
        {
            joint.enableCollision = false;
        }
        foreach (Collider collider in Colliders)
        {
            collider.enabled = false;
        }
        foreach (Rigidbody rigidbody in Rigidbodies)
        {
            rigidbody.detectCollisions = false;
            rigidbody.useGravity = false;
        }
    }
}
