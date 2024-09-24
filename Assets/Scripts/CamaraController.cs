using UnityEngine;

public class CamaraController : MonoBehaviour
{
    [SerializeField] private int currentCameraIndex = 0; 
    [SerializeField] private Material[] cameraMaterials; // Array of materials that show the camera views
    [SerializeField] private Material offMaterial;       // Material for the "off" state
    [SerializeField] private MeshRenderer monitorRenderer;   
    private bool[] cameraStates; 

    void Start(){
        monitorRenderer = GetComponent<MeshRenderer>();
        
        cameraStates = new bool[cameraMaterials.Length];
        for (int i = 0; i < cameraStates.Length; i++)
        {
            cameraStates[i] = true;
        }

        UpdateMonitorMaterial(currentCameraIndex); 
    }

    public void NextCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameraMaterials.Length;
        UpdateMonitorMaterial(currentCameraIndex);
    }

    public void PreviousCamera()
    {
        currentCameraIndex = (currentCameraIndex + 1) % cameraMaterials.Length;
        UpdateMonitorMaterial(currentCameraIndex);
    }

    public void ToggleCamera()
    {
        cameraStates[currentCameraIndex] = !cameraStates[currentCameraIndex];
        UpdateMonitorMaterial(currentCameraIndex);
    }

    private void UpdateMonitorMaterial(int currentCameraIndex)
    {
        if(cameraStates[currentCameraIndex]){ //on
            monitorRenderer.material = cameraMaterials[currentCameraIndex];
        }
        else{ //off
            monitorRenderer.material = offMaterial;
        }
    }
}
