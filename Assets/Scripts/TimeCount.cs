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
    public GameObject GameOverPanel;

    // Start is called before the first frame update
    void Start(){}

    // Update is called once per frame
    void Update()
    {
        /* Julien's idea
         * myTime = Time.realtimeSinceStartup;
         * timerText.text = myTime.ToString("f2");
         */

        myTime += Time.deltaTime; //myTime += Time.deltaTime;
        seconds = 60 - myTime % 60;
        minutes = 60 - myTime / 60;
        timerText.text = "Timer: " + string.Format("{0:00}:{1:00}", Mathf.FloorToInt(minutes), Mathf.FloorToInt(seconds));
        // hours = 1 - minutes / 60;
        // timerText.text = "Timer: " + string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(hours), Mathf.FloorToInt(minutes), Mathf.FloorToInt(seconds));

        if (hours == 0 && minutes == 0 && seconds == 0)
        {
            GameOverPanel.SetActive(true);
        }

        if (ScorePanel.activeSelf == true)
        {
            this.enabled = false;
            Debug.Log(timerText.text);
        }  
    }
}
