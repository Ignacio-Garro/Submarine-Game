using UnityEngine;
using UnityEngine.PlayerLoop;

public class moveHands : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float lerpSpeed = 2.0f; 
    Vector3 targetEulerAngles;

    private void FixedUpdate()
    {
        Quaternion targetRotation = target.transform.rotation;

        Quaternion adjustedRotation = targetRotation * Quaternion.Euler(-100f, 0f, 0f);

        transform.rotation = Quaternion.Slerp(transform.rotation, adjustedRotation, Time.deltaTime * lerpSpeed);
    }
}
