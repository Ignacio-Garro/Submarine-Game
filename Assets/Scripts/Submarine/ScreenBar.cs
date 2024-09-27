using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] Image insideBar;
    [SerializeField] Image errorScreen;
    [SerializeField] Image dangerIcon;
    [SerializeField] Gradient colorGradient;
    [SerializeField] float errorIconFlickduration;
    float percentage = 0;
    bool broken = false;
    public float Percentage => percentage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void SetBarPercentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 1);
        this.percentage = percentage;
        insideBar.fillAmount = percentage;
        insideBar.color = colorGradient.Evaluate(percentage);
        percentageText.text = Mathf.RoundToInt(percentage * 100) + "%";
    }

    public void Break()
    {
        broken = true;
        errorScreen.gameObject.SetActive(true);
        InvokeRepeating("FlickerError", 0f, errorIconFlickduration);
    }

    public void Fix()
    {
        broken = false;
        errorScreen.gameObject.SetActive(false);
        CancelInvoke("FlickerError");
    }

    public void FlickerError()
    {
        dangerIcon.gameObject.SetActive(!dangerIcon.gameObject.activeInHierarchy);
    }
}