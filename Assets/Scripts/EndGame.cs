using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* This script displays the score details at the end of the game. */
public class EndGame : MonoBehaviour
{
    [SerializeField] TMP_Text totalScoreText;

    private string finalTimeText;
    private GameManager manager;
    private TimeCount timeCount;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        timeCount = GameObject.Find("Time Text").GetComponent<TimeCount>();
    }

    // Update is called once per frame
    void Update()
    {
        finalTimeText = "Your time : " + timeCount.timerText.text[7..];
        totalScoreText.text = "Your score : " + manager.score.ToString() + "\n" + finalTimeText + "\n" + "Your mark : " + manager.playerMark + "/20";
    }
}
