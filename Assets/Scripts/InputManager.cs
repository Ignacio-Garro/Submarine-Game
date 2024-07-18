using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;


public class InputManager : MonoBehaviour
{

    public static InputManager Instance;

    public delegate void DelgateWithPlayerParam(GameObject player, Camera camera);

    public event DelgateWithPlayerParam onInteractPressed = (_,_) => { };
    public event DelgateWithPlayerParam onDropPressed = (_,_) => { };
    public event DelgateWithPlayerParam onClickPressed = (_,_) => { };
    public event DelgateWithPlayerParam onOnePressed = (_,_) => { };
    public event DelgateWithPlayerParam onTwoPressed = (_,_) => { };
    public event DelgateWithPlayerParam onThreePressed = (_,_) => { };

    [Header("info")]
    private Camera playerCamera;
    private GameObject actualPlayer;
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private Material interactMaterial = null;
    [SerializeField] private PlayerStats playerStats;

    IClickableObject clickedObject = null;
    GameObject viewedActor = null;
    Material previousMaterial = null;

    [Header("GrabItems")]
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;


    private IGrabbableObject grabbedObject = null;

    public Camera PlayerCamera { get => playerCamera; set => playerCamera = value; }
    public GameObject ActualPlayer { get => actualPlayer; set => actualPlayer = value; }

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }


   


    // Update is called once per frame
    void Update()
    {
        if (actualPlayer == null || playerCamera == null) return;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            if((actorChocado == null || actorChocado != viewedActor) && viewedActor != null)
            {
                viewedActor.GetComponent<Renderer>().material = previousMaterial;
            }
            if(actorChocado.GetComponent<IClickableObject>() != null || actorChocado.GetComponent<IInteractuableObject>() != null)
            {
                if (viewedActor != actorChocado)
                {
                    viewedActor = actorChocado;
                    previousMaterial = actorChocado.GetComponent<Renderer>().material;
                }
                actorChocado.GetComponent<Renderer>().material = interactMaterial;
            }
        }
        
    }


    private void OnInteract()
    {
        if (actualPlayer == null || playerCamera == null) return;
        TryToInteractWithObject();
        onInteractPressed(actualPlayer, playerCamera);
    }

    private void OnDrop()
    {
        if (actualPlayer == null || playerCamera == null) return;
        onDropPressed(actualPlayer, playerCamera);
    }

    private void OnOne()
    {
        if (actualPlayer == null || playerCamera == null) return;
        onOnePressed(actualPlayer, playerCamera);
    }
    private void OnTwo()
    {
        if (actualPlayer == null || playerCamera == null) return;
        onTwoPressed(actualPlayer, playerCamera);
    }
    private void OnThree()
    {
        if (actualPlayer == null || playerCamera == null) return;
        onThreePressed(actualPlayer, playerCamera);
    }

    private void TryToInteractWithObject()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            IInteractuableObject interactuableObject = actorChocado.GetComponent<IInteractuableObject>();
            if (interactuableObject != null)
            {
                interactuableObject.OnInteract(actualPlayer);
            }
        }
    }
    


    private void OnNewClickPressed()
    {
        if (actualPlayer == null || playerCamera == null) return;
        onClickPressed(actualPlayer, playerCamera);
    }

    private void OnNewClickRelease()
    {
        clickedObject?.OnClickRelease();
    }

    public void dropObject(){
        grabbedObject = null;
        playerStats.setGrabbedObject(null);
    }
    
}
