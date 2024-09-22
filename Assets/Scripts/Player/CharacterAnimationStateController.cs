using UnityEngine;

public class CharacterAnimationStateController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerMovement.MovementState currentState;
    void Start()
    {
        animator = GetComponent<Animator>();
        playerMovement = GetComponentInParent<PlayerMovement>();

        Debug.Log(animator);
        Debug.Log(playerMovement);

        if(playerMovement == null){
            Debug.Log ("no playerMovement");
        }
        else{
            Debug.Log ("YES playerMovement");
        }
        if(animator == null){
            Debug.Log ("no animator");
        }
        else{
            Debug.Log ("YES animator");
        }
    }

    void Update()
    {
        currentState = playerMovement.state;
        /*
        public enum MovementState {
        still,
        walking,
        sprinting,
        crouching,
        air,
        swimming,
        sprintswimming,
        ladder
        }
        */
        switch (currentState){
            case PlayerMovement.MovementState.still:
                currentState = PlayerMovement.MovementState.still;
                animator.SetInteger("state", 1);
                Debug.Log ("1");
                break;

            case PlayerMovement.MovementState.walking:
                currentState = PlayerMovement.MovementState.walking;
                animator.SetInteger("state", 2);
                Debug.Log ("2");
                break;

            case PlayerMovement.MovementState.sprinting:
                currentState = PlayerMovement.MovementState.sprinting;
                animator.SetInteger("state", 3);
                Debug.Log ("3");
                break;

            case PlayerMovement.MovementState.crouching:
                currentState = PlayerMovement.MovementState.crouching;
                animator.SetInteger("state", 4);
                Debug.Log ("4");
                break;

            case PlayerMovement.MovementState.air:
                currentState = PlayerMovement.MovementState.air;
                animator.SetInteger("state", 5);
                Debug.Log ("5");
                break;

            case PlayerMovement.MovementState.swimming:
                currentState = PlayerMovement.MovementState.swimming;
                animator.SetInteger("state", 6);
                Debug.Log ("6");
                break;

            case PlayerMovement.MovementState.sprintswimming:
                currentState = PlayerMovement.MovementState.sprintswimming;
                animator.SetInteger("state", 7);
                Debug.Log ("7");
                break;

            case PlayerMovement.MovementState.ladder:
                currentState = PlayerMovement.MovementState.ladder;
                animator.SetInteger("state", 8);
                Debug.Log ("8");
                break;

            default:
                Debug.Log("Unknown state");
                animator.SetInteger("state", -1);
                break;
        }
    }
}
