using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI speedText;
    [SerializeField] private Rigidbody rb;
    Vector3 velocity;

    private void Update() { 
        velocity = rb.velocity;
        // Exclude the Y-axis velocity
        velocity.y = 0f;
        // Calculate the magnitude of the velocity vector
        speedText.text = "Speed: " + velocity.magnitude.ToString("F2");;
    }
}
