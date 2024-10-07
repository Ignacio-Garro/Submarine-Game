using UnityEngine;

public class submarineWaterRenderer : MonoBehaviour{
    [SerializeField] private Camera camera;
    [SerializeField] private bool isInsideSubmarine = true;
    private int insideWaterLayer;
    private int outsideWaterLayer;

    void Start()
    {
        // Get the layer of the target GameObject
        insideWaterLayer = LayerMask.NameToLayer("insidePool");
        outsideWaterLayer = LayerMask.NameToLayer("outsidePool");

        //camera = GetComponentInChildren<Camera>();
    }
    public void EnterSubmarine(GameObject player){

        camera = player.GetComponent<Camera>();
        camera.cullingMask |= (1 << insideWaterLayer); //render
        camera.cullingMask &= ~(1 << outsideWaterLayer);//dont render

        isInsideSubmarine = true;
        Debug.Log("enter submarine");
    }
    public void ExitSubmarine(GameObject player){
        camera = player.GetComponent<Camera>();
        camera.cullingMask &= ~(1 << insideWaterLayer);//dont render
        camera.cullingMask |= (1 << outsideWaterLayer);//render

        isInsideSubmarine = false;
        Debug.Log("exit submarine");
    }
}
