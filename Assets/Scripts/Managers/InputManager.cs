using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{

    InputAction moveAction;
    InputAction lookAction; 

    public static InputManager Instance;

    public delegate void DelgateWithPlayerParam(GameObject player, Camera camera);


    public event DelgateWithPlayerParam onInteractPressed = (_, _) => { };
    public event DelgateWithPlayerParam onInteractReleased = (_, _) => { };
    public event DelgateWithPlayerParam onDropPressed = (_, _) => { };
    public event DelgateWithPlayerParam onClickPressed = (_, _) => { };
    public event DelgateWithPlayerParam onClickReleased = (_, _) => { };
    public event DelgateWithPlayerParam onOnePressed = (_, _) => { };
    public event DelgateWithPlayerParam onTwoPressed = (_, _) => { };
    public event DelgateWithPlayerParam onThreePressed = (_, _) => { };
    public event DelgateWithPlayerParam onJumpPressed = (_, _) => { };
    public event DelgateWithPlayerParam onJumpReleased = (_, _) => { };
    public event DelgateWithPlayerParam onCrouchPressed = (_, _) => { };
    public event DelgateWithPlayerParam onCrouchReleased = (_, _) => { };
    public event DelgateWithPlayerParam onSprintPressed = (_, _) => { };
    public event DelgateWithPlayerParam onSprintReleased = (_, _) => { };


    public event DelgateWithPlayerParam onInteractPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onInteractReleasedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onDropPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onClickPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onClickReleasedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onOnePressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onTwoPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onThreePressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onJumpPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onJumpReleasedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onCrouchPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onCrouchReleasedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onSprintPressedAfterBlock = (_, _) => { };
    public event DelgateWithPlayerParam onSprintReleasedAfterBlock = (_, _) => { };



    PlayerInput playerInput;
    


    [Header("info")]
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private float itemInteractionRange = 5f;
    [SerializeField] private Material interactuableMaterial = null;

    IInteractuableObject viewedActor = null;
    bool inputIsBlocked = false;
    public bool InputIsBlocked  {get => inputIsBlocked; set{inputIsBlocked = value; ReleaseItemUsage(ActualPlayer.GetComponent<PlayerInventory>().currentHoldingItem); } }


    private Vector2 moveInput => moveAction == null ? Vector2.zero : moveAction.ReadValue<Vector2>();
    private Vector2 lookInput => lookAction == null ? Vector2.zero : lookAction.ReadValue<Vector2>();
    public Vector2 MoveInputNormal => inputIsBlocked ? Vector2.zero : moveInput;
    public Vector2 LookInputNormal => inputIsBlocked ? Vector2.zero : lookInput;
    public Vector2 MoveInputBlock => inputIsBlocked ? moveInput : Vector2.zero;
    public Vector2 LookInputBlock => inputIsBlocked ? lookInput : Vector2.zero;


    public Camera PlayerCamera => GameManager.Instance.PlayerCamera;
    public GameObject ActualPlayer => GameManager.Instance.ActualPlayer;



    private GameObject currentWatchingObject = null;
    private ItemInteractuableInterface currentInteractingItemObject;

    private bool isUsingItem;
    

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["move"];
        lookAction = playerInput.actions["look"];
        InputAction jumpAction = playerInput.actions["jump"];
        InputAction crouchAction = playerInput.actions["crouch"];
        InputAction sprintAction = playerInput.actions["sprint"];
        InputAction interactAction = playerInput.actions["interact"];
        InputAction dropAction = playerInput.actions["drop"];
        InputAction oneAction = playerInput.actions["one"];
        InputAction twoAction = playerInput.actions["two"];
        InputAction threeAction = playerInput.actions["three"];
        InputAction clickAction = playerInput.actions["fire"];

        jumpAction.started += OnJumpPressedFunction;
        jumpAction.canceled += OnJumpReleasedFunction;
        crouchAction.started += OnCrouchPressedFunction;
        crouchAction.canceled += OnCrouchReleasedFunction;
        sprintAction.started += OnSprintPressedFunction;
        sprintAction.canceled += OnSprintReleasedFunction;
        interactAction.started += OnInteractPressedFunction;
        interactAction.canceled += OnInteractReleasedFunction;
        dropAction.started += OnDropPressedFunction;
        oneAction.started += OnOnePressedFunction;
        twoAction.started += OnTwoPressedFunction;
        threeAction.started += OnThreePressedFunction;
        clickAction.started += OnClickPressedFunction;
        clickAction.canceled += OnClickReleasedFunction;
    }

    // Update is called once per frame
    void Update()
    {

        GetWatchingObject();
        CheckForItemInteraction();



        //Call on interactuable objects when they are in range
        if (GameManager.Instance == null || ActualPlayer == null || PlayerCamera == null) return;
        
            IInteractuableObject inter = GetWatchingComponent<IInteractuableObject>();
            if(inter == null)
            {
                if(viewedActor != null)
                {
                    viewedActor.OnExitInRange();
                    viewedActor = null;
                }
            }
            else
            {
                if(viewedActor == null)
                {
                    inter.OnEnterInRange();
                    viewedActor = inter;
                }
                else if(viewedActor != inter)
                {
                    viewedActor.OnExitInRange();
                    inter.OnEnterInRange();
                    viewedActor = inter;
                }
            }
        
    }

    private void GetWatchingObject()
    {
        if (PlayerCamera == null) return;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            currentWatchingObject = hit.collider.gameObject;
        }
        else
        {
            currentWatchingObject = null;
        }
    }

    private void CheckForItemInteraction()
    {
        if (!isUsingItem) return;
        ItemInteractuableInterface interactuableObject = GetWatchingComponent<ItemInteractuableInterface>();
        if (interactuableObject != currentInteractingItemObject)
        {
            PlayerInventory inventory = ActualPlayer.GetComponent<PlayerInventory>();
            currentInteractingItemObject?.OnStopInteracting(inventory == null ? null : inventory.currentHoldingItem);
            interactuableObject?.OnEnterInteractionRange(inventory == null ? null : inventory.currentHoldingItem);
            currentInteractingItemObject = interactuableObject; 
        }
    }

    private T GetWatchingComponent<T>()
    {
        if (currentWatchingObject == null) return default(T);
        GameObject aux = currentWatchingObject;
        T searchedComponent = aux.GetComponent<T>();
        while (searchedComponent == null && aux.transform.parent != null)
        {
            aux = aux.transform.parent.gameObject;
            searchedComponent = aux.GetComponent<T>();
        }
        return searchedComponent;
    }

    public void StopUsingItem(ItemPickable item)
    {
        item?.GetComponent<ItemFunctionInterface>()?.OnItemRemove();
        currentInteractingItemObject?.OnStopInteracting(item);
        currentInteractingItemObject = null;
        isUsingItem = false;
    }

    public void ReleaseItemUsage(ItemPickable item)
    {
        item?.GetComponent<ItemFunctionInterface>()?.OnItemUnuse();
        currentInteractingItemObject?.OnStopInteracting(item);
        currentInteractingItemObject = null;
        isUsingItem = false;
    }

    private void OnJumpPressedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onJumpPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onJumpPressed(ActualPlayer, PlayerCamera);
    }

    private void OnJumpReleasedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onJumpReleasedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onJumpReleased(ActualPlayer, PlayerCamera);
    }

    private void OnClickPressedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onClickPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onClickPressed(ActualPlayer, PlayerCamera);
        TryToUseItem();
    }

    private void OnClickReleasedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null) return;
        
        if (inputIsBlocked)
        {
            onClickReleasedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onClickReleased(ActualPlayer, PlayerCamera);
        if (ActualPlayer.GetComponent<PlayerInventory>() != null)
        {
            ReleaseItemUsage(ActualPlayer.GetComponent<PlayerInventory>().currentHoldingItem);
        }
    }


    private void OnCrouchPressedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onCrouchPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onCrouchPressed(ActualPlayer, PlayerCamera);
    }

    private void OnCrouchReleasedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onCrouchReleasedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onCrouchReleased(ActualPlayer, PlayerCamera);
    }

    private void OnSprintPressedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onSprintPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onSprintPressed(ActualPlayer, PlayerCamera);
    }

    private void OnSprintReleasedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onSprintReleasedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onSprintReleased(ActualPlayer, PlayerCamera);
    }

    private void OnInteractPressedFunction(InputAction.CallbackContext context)
    {
        if (inputIsBlocked)
        {
            onInteractPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        if (ActualPlayer == null || PlayerCamera == null) return;
        TryToInteractWithObject();
        onInteractPressed(ActualPlayer, PlayerCamera);
    }

    private void OnInteractReleasedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        if (inputIsBlocked)
        {
            onInteractReleasedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onInteractReleased(ActualPlayer, PlayerCamera);
    }

    private void OnDropPressedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null || PlayerCamera == null) return;

        if (inputIsBlocked)
        {
            onDropPressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }

        onDropPressed(ActualPlayer, PlayerCamera);
    }

    private void OnOnePressedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        if (inputIsBlocked)
        {
            onOnePressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        
        onOnePressed(ActualPlayer, PlayerCamera);
    }
    private void OnTwoPressedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        if(inputIsBlocked)
        {
            onTwoPressedAfterBlock(ActualPlayer, PlayerCamera);
        }
        onTwoPressed(ActualPlayer, PlayerCamera);
    }
    private void OnThreePressedFunction(InputAction.CallbackContext context)
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        if (inputIsBlocked)
        {
            onThreePressedAfterBlock(ActualPlayer, PlayerCamera);
            return;
        }
        onThreePressed(ActualPlayer, PlayerCamera);
    }

    private void TryToInteractWithObject()
    {
        IInteractuableObject interactuableObject = GetWatchingComponent<IInteractuableObject>();
        if (interactuableObject != null)
        {
            interactuableObject.OnInteract(ActualPlayer);
        }
    }
      

    private void TryToUseItem()
    {
        if(ActualPlayer == null || PlayerCamera == null) return;
        ItemPickable currentItem = ActualPlayer.GetComponent<PlayerInventory>() != null ? ActualPlayer.GetComponent<PlayerInventory>().currentHoldingItem : null;
        if (currentItem == null) return;
        currentItem.gameObject.GetComponent<ItemFunctionInterface>()?.OnItemUse();
        isUsingItem = true;
        ItemInteractuableInterface interactuableObject = GetWatchingComponent<ItemInteractuableInterface>();
        if (interactuableObject != null)
        {
            interactuableObject.OnInteract(currentItem);
            currentInteractingItemObject = interactuableObject;
        }
    }



    public void AddInteractuableMaterial(GameObject obj)
    {
        /*Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            List<Material> materials = render.materials.ToList();
            materials.Add(interactuableMaterial);
            render.materials = materials.ToArray();
        }*/
    }
    public void RemoveInteractuableMaterial(GameObject obj)
    {
        /*Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            List<Material> materials = render.materials.ToList();
            materials.Remove(interactuableMaterial);
            render.materials = materials.ToArray();
        }*/
    }
}
