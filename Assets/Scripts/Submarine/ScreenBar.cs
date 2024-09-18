using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] RectTransform insideBar;
    [SerializeField] Image errorScreen;
    [SerializeField] Image dangerIcon;
    [SerializeField] Color greenColor;
    [SerializeField] Color yellowColor;
    [SerializeField] Color redColor;
    [SerializeField] Color blackColor;
    [SerializeField] int greenPoint;
    [SerializeField] int yellowPoint;
    [SerializeField] int redPoint;
    [SerializeField] float errorIconFlickduration;
    float height = 0;
    float percentage = 0;
    bool broken = false;
    public float Percentage => percentage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        height = insideBar.sizeDelta.y;
    }

    public void SetBarPorcentage(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);
        this.percentage = percentage;
        Vector2 size = insideBar.sizeDelta;
        
        size.y = Mathf.Lerp(0, height, percentage / 100f);

        insideBar.sizeDelta = size;

        Image image = insideBar.GetComponent<Image>();
        if (percentage < greenPoint)
        { 
            image.color = greenColor;
        }
        else if (percentage < yellowPoint)
        {
            image.color = Color.Lerp(greenColor, yellowColor, (percentage - greenPoint) / (yellowPoint - greenPoint));
        }
        else if(percentage < redPoint)
        {
            image.color = Color.Lerp(yellowColor, redColor, (percentage - yellowPoint) / (redPoint - yellowPoint));
        }
        else
        {
            image.color = Color.Lerp(redColor, blackColor, (percentage - redPoint) / (100 - redPoint));
        }
        percentageText.text = Mathf.RoundToInt(percentage) + "%";
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
        errorScreen.gameObject.SetActive(true);
        CancelInvoke("FlickerError");
    }

    public void FlickerError()
    {
        dangerIcon.gameObject.SetActive(!dangerIcon.gameObject.activeInHierarchy);
    }
}
