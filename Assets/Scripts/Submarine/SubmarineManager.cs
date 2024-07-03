using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineManager : MonoBehaviour
{
    // Start is called before the first frame update

    public static SubmarineManager Instance;
    [SerializeField] MonoBehaviour button1;
    public  MonoBehaviour Button1 => button1;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
