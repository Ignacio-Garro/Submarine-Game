using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : MonoBehaviour
{

    [SerializeField] private MonoBehaviour playerCamera;
    [SerializeField] private MonoBehaviour actualPlayer;
    [SerializeField] private float interactionRange = 5.0f;

    IClickableObject clickedObject = null;

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
            if(actorChocado.GetComponent<IClickableObject>() != null)
            {
                Debug.Log("Pulsa click para interactuar con: " + actorChocado.name);
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
