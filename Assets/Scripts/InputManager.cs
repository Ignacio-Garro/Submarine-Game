using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private Material interactuableMaterial = null;

    IInteractuableObject viewedActor = null;

    


   

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

        //Call on interactuable objects when they are in range
        if (GameManager.Instance == null || ActualPlayer == null || PlayerCamera == null) return;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out RaycastHit hit, interactionRange))
        {
            GameObject actorChocado = hit.collider.gameObject;
            IInteractuableObject inter = actorChocado.GetComponent<IInteractuableObject>();
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

    public void AddInteractuableMaterial(GameObject obj)
    {
        Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            List<Material> materials = render.materials.ToList();
            materials.Add(interactuableMaterial);
            render.materials = materials.ToArray();
        }
    }
    public void RemoveInteractuableMaterial(GameObject obj)
    {
        Renderer[] renders = obj.GetComponentsInChildren<Renderer>();
        foreach (Renderer render in renders)
        {
            List<Material> materials = render.materials.ToList();
            materials.Remove(interactuableMaterial);
            render.materials = materials.ToArray();
        }
    }
}
