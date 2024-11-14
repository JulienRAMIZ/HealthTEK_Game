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
    private GridScript grid;
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
       // finalTimeText = "Your time : " + timeCount.timerText.text[7..];
        //finalTimeText = "Your time : " + (15 - int.Parse(timeCount.timerText.text.Substring(6, 3))).ToString() + ":" + (60 - int.Parse(timeCount.timerText.text[10..])).ToString();
        finalTimeText = "Your time : " + timeCount.timerText.text;
        //totalScoreText.text = "Your score : " + manager.score.ToString() + "\n" + finalTimeText + "\n" + "Your mark : " + manager.playerMark + "/20";
        totalScoreText.text = "Your score :  \n Unit 2 : " + manager.unit[0] + " / 5 \n Unit 3 : " + manager.unit[1] + " / 5 \n Unit 4 : " + manager.unit[2] + " / 5 \n Unit 5 : " + manager.unit[3] + " / 5 \n Unit 6 : " + manager.unit[4] + " / 5 \n Unit 7 : " + manager.unit[5] + " / 5";
    }

    //public void CountScore()
    //{
    //    //Debug.Log("début");
    //    for (int i = 0; i < grid._height ; i++)
    //    {
    //        //Debug.Log("On est dans les i : " + i );
    //        for (int j = 0; j < grid._width ; j++)
    //        {
    //            //Debug.Log("On est dans les j : " + j);
    //            if (grid._listTiles[j, i].tag == "OpenedDoor")
    //            {
    //                //Debug.Log("Cette room est juste  :  x " + j + "y " + i + "  " + grid._listTiles[j, i].tag);
    //                manager.unit[i]++;
    //            }
    //        }
    //    }
    //}
    
        
    
    public void OpenAlud(string Urlname)
    {
        Application.OpenURL(Urlname);
    }
}
