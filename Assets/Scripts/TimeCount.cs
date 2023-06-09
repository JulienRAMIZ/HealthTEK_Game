using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* Script for the creation and the displaying of the timer */
public class TimeCount : MonoBehaviour
{
    private float myTime;
    private float seconds = 0.0f;
    private float minutes = 0.0f;
    private float hours = 0.0f;

    public TMP_Text timerText;
    public GameObject ScorePanel;

    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        /* Julien's idea
         * myTime = Time.realtimeSinceStartup;
         * timerText.text = myTime.ToString("f2");
         */

        myTime += Time.deltaTime;
        seconds = myTime % 60;
        minutes = myTime / 60;
        hours = minutes / 60;
        timerText.text = "Timer: " + string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(hours), Mathf.FloorToInt(minutes), Mathf.FloorToInt(seconds));

        if (ScorePanel.activeSelf == true)
        {
            this.enabled = false;
            Debug.Log(timerText.text);
        }
    }
}
