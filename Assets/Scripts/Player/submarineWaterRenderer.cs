using UnityEngine;

public class submarineWaterRenderer : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject insideWater;
    [SerializeField] private GameObject outsideWater;
    [SerializeField] private bool isInsideSubamrine = true;
    private int insideWaterLayer;
    private int outsideWaterLayer;

    void Start()
    {
        // Get the layer of the target GameObject
        insideWaterLayer = insideWater.layer;
        outsideWaterLayer = outsideWater.layer;

        camera = GetComponent<Camera>();
    }

    void Update()
    {
        if (isInsideSubamrine)
        {
            camera.cullingMask |= (1 << insideWaterLayer);

            camera.cullingMask &= ~(1 << outsideWaterLayer);
        }
        else
        {
            camera.cullingMask &= ~(1 << insideWaterLayer);

            camera.cullingMask |= (1 << outsideWaterLayer);
        }
    }
}
