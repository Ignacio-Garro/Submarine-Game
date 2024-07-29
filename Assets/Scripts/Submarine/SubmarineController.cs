
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Networking;

public class SubmarineController : NetworkBehaviour
{

    struct SubmarineEvent
    {
        public Action submarineEventAction;
        public float weigth;
        public SubmarineEvent(Action submarineEventAction, float weigth)
        {
            this.submarineEventAction = submarineEventAction;
            this.weigth = weigth;
        }
    }

    [Header("Submarine components")]
    public SubmarineMovement movement;
    public SinkingController sinking;
    public Engine submarineEngine;
    public List<SinkingHole> holes;


    [Header("Submarine settings")]
    [SerializeField] private Transform EnterPosition;


    private bool eventsShouldFire = true;
    private int sinkingRate = 0;
    private int drainRate = 0;
    public int SinkingRate => sinkingRate;
    public int DrainRate => drainRate;
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
    //    StartCoroutine(StartRecursiveEvent());
    }

    private void AddSubmarineEvents()
    {
        submarineRandomEvents.Add(new SubmarineEvent(RandomHoleOpens, 1));
    }

    private IEnumerator StartRecursiveEvent()
    {
        if (!IsServer) yield break;
        yield return new WaitForSeconds(UnityEngine.Random.Range(30, 120));
        if(!eventsShouldFire)
        {
            yield return new WaitWhile(() => !eventsShouldFire);
        }
        else
        {
            SelectRandomEvent()();
        }
        StartCoroutine(StartRecursiveEvent());

    }

    private Action SelectRandomEvent()
    {
        float totalWeigth = 0;
        submarineRandomEvents.ForEach(element => totalWeigth += element.weigth);
        float selectedWeigth = UnityEngine.Random.Range(0f, totalWeigth);
        float currentWeigth = 0;
        foreach (SubmarineEvent element in submarineRandomEvents)
        {
            currentWeigth += element.weigth;
            if (selectedWeigth <= currentWeigth)
            {
                return element.submarineEventAction;
            }
        }
        return submarineRandomEvents.Any() ? submarineRandomEvents.First().submarineEventAction : null;
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
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.Sleep();
                rb.transform.position = EnterPosition.position;
            }
            
        }
       
    }

    public void InsertCoal(GameObject coal)
    {
        if (!IsServer) return;
        submarineEngine.RefillEnginefuel(1);
        NetworkObject coalNetwork = coal.GetComponent<NetworkObject>();
        Assert.IsNotNull(coalNetwork);
        coalNetwork.Despawn();
    }


}
