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

    private ObjectGrabbable objectGrabbable;


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
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            actorChocado.GetComponent<IInteractuableObject>()?.OnInteract(actualPlayer);
        }

        //PARA AGARRAR OBJECTO
        if(objectGrabbable == null){
            Debug.Log("object1");
            //not carrying an object, try to grab
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit, interactionRange)){
                Debug.Log("object2");
                if(raycastHit.transform.TryGetComponent(out ObjectGrabbable objectGrabbable)){
                    objectGrabbable.Grab(objectGrabPointTransform);
                    this.objectGrabbable = objectGrabbable;
                    Debug.Log("object3");
                }
            }
        }
        else{
            //currently carrying somehting, drop
            objectGrabbable.Drop(objectGrabPointTransform);
            objectGrabbable = null;
        }
    }
    private void OnNewClickPressed()
    {
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
