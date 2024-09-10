
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

enum SubmarineEventType
{
    OpenHole
}

public class SubmarineController : NetworkBehaviour
{

    struct SubmarineEvent
    {
        public SubmarineEventType type;
        public Action submarineEventAction;
        public Func<float> weigth;
        public SubmarineEvent(SubmarineEventType type, Action submarineEventAction, Func<float> weigth)
        {
            this.type = type;
            this.submarineEventAction = submarineEventAction;
            this.weigth = weigth;
        }
    }

    [Header("Submarine components")]
    public SubmarineMovement movement;
    public SinkingController sinking;
    public Engine submarineEngine;
    public List<SinkingHole> holes;
    public submarineCollisionController collision;
    public SubmarineReactor reactor;
    public Lever MovementLever;
    public Lever FloatLever;

    [Header("Submarine settings")]
    [SerializeField] private Transform EnterPosition;


    //Global variables
    [Header("Global submarine variables")]

    [SerializeField]
    public float SubmarineGravity = 9.8f;
    [SerializeField]
    public float WaterHeigth = 0;
    [SerializeField]
    public float WaterDensity = 1000;
    [SerializeField]
    public float SubmarineLength = 10;

    bool currentPlayerIsInSubmarine = false;
    private bool eventsShouldFire = true;
    private int sinkingRate = 0;
    private int drainRate = 0;
    public int SinkingRate => sinkingRate;
    public int DrainRate => drainRate;
    private float pressure => (WaterHeigth - transform.position.y) * WaterDensity * SubmarineGravity;



    List<SubmarineEvent> submarineRandomEvents = new List<SubmarineEvent>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

  
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        foreach (SinkingHole hole in holes)
        {
            hole.turnOffServerCallback += () => sinkingRate--;
            hole.turnOnServerCallback += () => sinkingRate++;
        }
        AddSubmarineEvents();
        StartCoroutine(StartRecursiveEvent());
    }

    private void AddSubmarineEvents()
    {
        submarineRandomEvents.Add(new SubmarineEvent(SubmarineEventType.OpenHole, RandomHoleOpens, () => Mathf.Clamp((pressure - 2) / 2, 0, 3)));
    }

    private IEnumerator StartRecursiveEvent()
    {
        if (!IsServer) yield break;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1, 2));
        if(!eventsShouldFire)
        {
            yield return new WaitWhile(() => !eventsShouldFire);
        }
        else
        {
            SelectRandomEvent()?.Invoke();
        }
        StartCoroutine(StartRecursiveEvent());

    }

    private Action SelectRandomEvent()
    {
        float totalWeigth = 0;
        submarineRandomEvents.ForEach(element => totalWeigth += element.weigth());
        if (totalWeigth == 0) return null;
        float selectedWeigth = UnityEngine.Random.Range(0f, totalWeigth);
        float currentWeigth = 0;
        foreach (SubmarineEvent element in submarineRandomEvents)
        {
            currentWeigth += element.weigth();
            if (selectedWeigth <= currentWeigth)
            {
                return element.submarineEventAction;
            }
        }
        return null;
    }


    //-----------------------------Random Events that can occur in submarine------------------------
    
    public void RandomHoleOpens()
    {
        if (!IsServer) return;
        List<SinkingHole> closedHoles = holes.FindAll((SinkingHole hole) => !hole.HoleIsOpen);
        if (closedHoles.Count == 0) return;
        closedHoles[UnityEngine.Random.Range(0, closedHoles.Count)].OpenHoleFromServer();
    }


    //-----------------------------Events caused by clients-----------------------------------------
    




    public void EnterSubmarine(GameObject player)
    {
        if (IsServer)
        {
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.Sleep();
                player.transform.parent = gameObject.transform;
            }
        }
        if (IsClient)
        {
            if(player == GameManager.Instance.ActualPlayer) currentPlayerIsInSubmarine = true;
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.Sleep();
                rb.transform.position = EnterPosition.position;
            }
            if(player == GameManager.Instance.ActualPlayer)
            {
                PlayerController controller = player.GetComponent<PlayerController>();
                if(controller != null)
                {
                    controller.EnterSubmarine();
                }
            }
            
            
        }
       
    }

    public void StartPumping()
    {
        drainRate += 2;
    }

    public void StopPumping()
    {
        drainRate -= 2;
    }


}
