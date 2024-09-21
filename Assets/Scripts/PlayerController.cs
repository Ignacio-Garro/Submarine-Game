using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] GameObject waterMask;
    public void EnterSubmarine()
    {
        //waterMask.SetActive(true);
    }
    public void ExitSubmarine()
    {
        //waterMask.SetActive(false);
    }
}
