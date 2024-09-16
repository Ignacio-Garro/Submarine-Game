using System;
using UnityEngine;

public class ProceduralAnimator : MonoBehaviour
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
                isMorphing = false;
                isMorphed = true;
                onMorphed();
            }
        }
        if (isDemorphing)
        {
            morphableMesh.SetBlendShapeWeight(0, morphableMesh.GetBlendShapeWeight(0) - (100 / morphTime) * Time.deltaTime);
            if (morphableMesh.GetBlendShapeWeight(0) <= 0)
            {
                isDemorphing = false;
                isDemorphed = true;
                onDemorphed();
            }
        }
    }

    public void Morph(Action onMorphed)
    {
        if (!isDemorphed) return;
        this.onMorphed = onMorphed;
        isMorphing = true;
        isDemorphed = false;
    }

    public void Demorph(Action onDemorphed)
    {
        if (!isMorphed) return;
        this.onDemorphed = onDemorphed;
        isDemorphing = true;
        isMorphed = false;
    }


    
}
