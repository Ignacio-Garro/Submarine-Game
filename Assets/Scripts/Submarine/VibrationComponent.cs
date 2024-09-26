using Unity.Netcode;
using UnityEngine;

public class VibrationComponent : NetworkBehaviour
{

    [SerializeField] float amplitud = 0.1f;
    // Frecuencia de la vibración (controla qué tan rápido varía el ruido)
    [SerializeField] float frecuencia = 1f;

    // Posición original del objeto
    private Vector3 posicionOriginal;

    // Variables para el offset en Perlin Noise
    private float offsetX;
    private float offsetY;

    bool isActive = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        posicionOriginal = transform.localPosition;
        offsetX = Random.Range(0f, 100f);
        offsetY = Random.Range(0f, 100f);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive || !IsServer) return;
        float desplazamientoX = (Mathf.PerlinNoise(Time.time * frecuencia + offsetX, 0) - 0.5f) * 2f * amplitud;
        float desplazamientoY = (Mathf.PerlinNoise(0, Time.time * frecuencia + offsetY) - 0.5f) * 2f * amplitud;
        transform.localPosition = posicionOriginal + new Vector3(desplazamientoX, desplazamientoY, 0);
    }

    public void SetActiveVibration(bool active)
    {
        isActive = active;
    }

}
