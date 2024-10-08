using System;
using Unity.Netcode;
using UnityEngine;

public class ProceduralAnimator : NetworkBehaviour
{

    [SerializeField] SkinnedMeshRenderer morphableMesh;
    [SerializeField] float morphTime = 1f;

    bool isMorphing = false;
    bool isMorphed = false;
    bool isDemorphing = false;
    bool isDemorphed = true;
    Action onMorphed;
    Action onDemorphed;

    void Update()
    {
        if (isMorphing)
        {
            morphableMesh.SetBlendShapeWeight(0, morphableMesh.GetBlendShapeWeight(0) + (100 / morphTime) * Time.deltaTime);
            if(morphableMesh.GetBlendShapeWeight(0) >= 100)
            {
                if (IsServer)
                {
                    StopMorphClientRpc();
                    onMorphed();
                }
                else
                {
                    morphableMesh.SetBlendShapeWeight(0, 100);
                }
            }
        }
        if (isDemorphing)
        {
            morphableMesh.SetBlendShapeWeight(0, morphableMesh.GetBlendShapeWeight(0) - (100 / morphTime) * Time.deltaTime);
            if (morphableMesh.GetBlendShapeWeight(0) <= 0)
            {
                if (IsServer)
                {
                    StopDemorphClientRpc();
                    onDemorphed();
                }
                else
                {
                    morphableMesh.SetBlendShapeWeight(0, 0);
                }
            }
        }
    }
    [ClientRpc(RequireOwnership = false)]
    public void StartMorphClientRpc()
    {
        isMorphing = true;
        isDemorphed = false;
    }
    [ClientRpc(RequireOwnership = false)]
    public void StopMorphClientRpc()
    {
        morphableMesh.SetBlendShapeWeight(0, 100);
        isMorphing = false;
        isMorphed = true;
    }

    [ClientRpc(RequireOwnership = false)]
    public void StartDemorphClientRpc()
    {
        isDemorphing = true;
        isMorphed = false;
    }
    [ClientRpc(RequireOwnership = false)]
    public void StopDemorphClientRpc()
    {
        morphableMesh.SetBlendShapeWeight(0, 0);
        isDemorphing = false;
        isDemorphed = true;
    }

    public void Morph(Action onMorphed)
    {
        if (!isDemorphed) return;
        this.onMorphed = onMorphed;
        StartMorphClientRpc();
    }

    public void Demorph(Action onDemorphed)
    {
        if (!isMorphed) return;
        this.onDemorphed = onDemorphed;
        StartDemorphClientRpc();
    }

}
