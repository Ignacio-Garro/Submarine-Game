using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class Bar : MonoBehaviour
{
    [SerializeField] Image insideBar;
    [SerializeField] Gradient colorGradient;
    [SerializeField] bool shouldAlignWithCamera = true;
    float percentage = 0;
    bool broken = false;
    public float Percentage => percentage;

    public void SetBarPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);
        this.percentage = percentage;
        insideBar.fillAmount = percentage / 100f;
        insideBar.color = colorGradient.Evaluate(percentage / 100);
    }

    private void Update()
    {
        if (shouldAlignWithCamera)
        {
            transform.LookAt(GameManager.Instance.ActualPlayer.transform);
        }
    }
}
