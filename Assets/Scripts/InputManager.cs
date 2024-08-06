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

    public Camera PlayerCamera => GameManager.Instance.PlayerCamera;
    public GameObject ActualPlayer => GameManager.Instance.ActualPlayer;

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
        if (GameManager.Instance == null || ActualPlayer == null || PlayerCamera == null) return;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, interactionRange))
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
        if (ActualPlayer == null || PlayerCamera == null) return;
        TryToInteractWithObject();
        onInteractPressed(ActualPlayer, PlayerCamera);
    }

    private void OnDrop()
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        onDropPressed(ActualPlayer, PlayerCamera);
    }

    private void OnOne()
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        onOnePressed(ActualPlayer, PlayerCamera);
    }
    private void OnTwo()
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        onTwoPressed(ActualPlayer, PlayerCamera);
    }
    private void OnThree()
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        onThreePressed(ActualPlayer, PlayerCamera);
    }

    private void TryToInteractWithObject()
    {
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            IInteractuableObject interactuableObject = actorChocado.GetComponent<IInteractuableObject>();
            while (interactuableObject == null && actorChocado.transform.parent != null)
            {
                actorChocado = actorChocado.transform.parent.gameObject;
                interactuableObject = actorChocado.GetComponent<IInteractuableObject>();
            }
            if (interactuableObject != null)
            {
                interactuableObject.OnInteract(ActualPlayer);
            }
        }
    }
    


    private void OnNewClickPressed()
    {
        if (ActualPlayer == null || PlayerCamera == null) return;
        onClickPressed(ActualPlayer, PlayerCamera);
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
