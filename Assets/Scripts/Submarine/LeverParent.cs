using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
using static UnityEngine.Rendering.HableCurve;

public abstract class LeverParent : NetworkBehaviour
{

    [SerializeField] float initialProgress = 0;
    [SerializeField] int segments = 5;

    //Value from 0 to 1
    protected NetworkVariable<float> progress = new NetworkVariable<float>(0);

    public float Progress => progress.Value;

    public override void OnNetworkSpawn()
    {
        progress.Value = initialProgress;
    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdateProgressServerRpc(float progress)
    {
        this.progress.Value = ApproximateLeverToSegment(progress);
    }

    public float ApproximateLeverToSegment(float progress)
    {
        progress = Mathf.Clamp01(progress);
        // Calcular el segmento al que pertenece el valor
        float segmentSize = 1f / (segments - 1);
        float aproximation = Mathf.Round(progress / segmentSize) * segmentSize;
        return aproximation;
    }

}
