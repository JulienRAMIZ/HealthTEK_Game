using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimeCount : MonoBehaviour
{
    private float myTime;
    [SerializeField] TMP_Text timerText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        myTime = Time.realtimeSinceStartup;
        timerText.text = myTime.ToString("f2");

        
    }
}
