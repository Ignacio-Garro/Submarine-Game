using UnityEngine;

public class SpotlightController : MonoBehaviour
{
    private Light spotlight;

    private Color defaultColor;
    private float defaultIntensity;

    private bool isLightOn = true;
    private bool isRedAndDim = false;

    [SerializeField] private Color emergencyLightColor = Color.red;
    [SerializeField] private float emergencyLightIntensity = 0.5f;

    void Start()
    {
        spotlight = GetComponentInChildren<Light>();
        defaultColor = spotlight.color;
        defaultIntensity = spotlight.intensity;
    }
    public void ToggleLight()
    {
        isLightOn = !isLightOn;
        spotlight.enabled = isLightOn;
    }

    public void ToggleRedAndDim()
    {
        if (isRedAndDim)
        {
            spotlight.color = defaultColor;
            spotlight.intensity = defaultIntensity;
        }
        else
        {
            spotlight.color = emergencyLightColor;
            spotlight.intensity = defaultIntensity * emergencyLightIntensity; // Dim the light by 50%
        }
        isRedAndDim = !isRedAndDim;
    }
}
