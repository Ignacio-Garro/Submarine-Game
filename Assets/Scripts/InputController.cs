using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{
    [Header("info")]
    [SerializeField] private MonoBehaviour playerCamera;
    [SerializeField] private MonoBehaviour actualPlayer;
    [SerializeField] private float interactionRange = 5.0f;
    [SerializeField] private Material interactMaterial = null;

    IClickableObject clickedObject = null;
    GameObject viewedActor = null;
    Material previousMaterial = null;

    [Header("GrabItems")]
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;


    private IGrabbableObject grabbedObject = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
        if (grabbedObject != null)
        {
            grabbedObject.OnDrop(actualPlayer);
            grabbedObject = null;
            return;
        }
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            IInteractuableObject interactuableObject = actorChocado.GetComponent<IInteractuableObject>();
            IGrabbableObject grabbableObject = actorChocado.GetComponent<IGrabbableObject>();
            if (interactuableObject != null)
            {
                interactuableObject.OnInteract(actualPlayer);
            }
            else if(actorChocado.GetComponent<IGrabbableObject>() != null)
            {
                grabbableObject.OnGrab(actualPlayer);
                grabbedObject = grabbableObject;
            }
        }
    }

    



    private void OnNewClickPressed()
    {
        if (grabbedObject != null)
        {
            return;
        }
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            actorChocado.GetComponent<IClickableObject>()?.OnClick(actualPlayer);
            clickedObject = actorChocado.GetComponent<IClickableObject>();
        }
    }

    private void OnNewClickRelease()
    {
        clickedObject?.OnClickRelease();
    }
}
