using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScreenBar : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI percentageText;
    [SerializeField] RectTransform insideBar;
    [SerializeField] Color greenColor;
    [SerializeField] Color yellowColor;
    [SerializeField] Color redColor;
    [SerializeField] Color blackColor;
    [SerializeField] int greenPoint;
    [SerializeField] int yellowPoint;
    [SerializeField] int redPoint;
    float height = 0;
    int percentage = 0;
    public int Percentage => percentage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        height = insideBar.sizeDelta.y;
    }

    void SetBarPorcentage(int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);
        this.percentage = percentage;
        Vector2 size = insideBar.sizeDelta;
        size.y = Mathf.Lerp(0, height, (float)percentage / 100);

        insideBar.sizeDelta = size;

        Image image = insideBar.GetComponent<Image>();
        if (percentage < greenPoint)
        { 
            image.color = greenColor;
        }
        else if (percentage < yellowPoint)
        {
            image.color = Color.Lerp(greenColor, yellowColor, (float)(percentage - greenPoint) / (yellowPoint - greenPoint));
        }
        else if(percentage < redPoint)
        {
            image.color = Color.Lerp(yellowColor, redColor, (float)(percentage - yellowPoint) / (redPoint - yellowPoint));
        }
        else
        {
            image.color = Color.Lerp(redColor, blackColor, (float)(percentage - redPoint) / (100 - redPoint));
        }
        percentageText.text = percentage + "%";
    }
}
