using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUD : MonoBehaviour
{
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public float timer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        timer = 500000;
    }

    // Update is called once per frame
    void Update()
    {
        float timer_10 = timer/100;
        if (Mathf.Approximately(timer_10, Mathf.RoundToInt(timer_10)))
        {
            countText.text = timer_10.ToString();
        }
        timer = timer - 1;
    }


}
