using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/* This script displays the score details at the end of the game. */
public class EndGame : MonoBehaviour
{

    [SerializeField] TMP_Text totalScoreText;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text resultTitle;
    [SerializeField] TMP_Text gameScoreText;

    private string finalTimeText;
    private GameManager manager;
    private GridScript grid;
    private TimeCount timeCount;
    private string[] unitName = new string[6] { "Engineering applied to orthopaedics as an example of the use of biomechanics biomaterials ", "Biosensors and medical devices based on electronic principles", "Surgical Instruments", "Artificial Intelligence and Health", "Anatomy and Physiology", "Genetics and genomics" };
    private bool resultShowed = false;
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
        totalScoreText.text = "Your score line by line  :  \n Line 1 : " + manager.unit[0] + " / 5 \n Line 2 : " + manager.unit[1] + " / 5 \n Line 3 : " + manager.unit[2] + " / 5 \n Line 4 : " + manager.unit[3] + " / 5 \n Line 5 : " + manager.unit[4] + " / 5 \n Line 6 : " + manager.unit[5] + " / 5";
        ScoreSentences();
        Result();

    }


    public void Result()
    {
        if(!resultShowed)
        {
            for (int i = 0; i < manager.unit.Length; i++)
            {
                if (manager.unit[i] >= 3)
                {
                    resultText.text = resultText.text + " \n • " + unitName[i];
                }
            }
            if (string.IsNullOrEmpty(resultText.text))
            {
                resultTitle.text = " You don't have the required knowledge for now \n But don't worry you can try again if you want ! ";
            }
            resultShowed = true;
        }

    }
        
    public void ScoreSentences()
    {
        if (!resultShowed)
        {
            if (manager.score >= 300)
            {
                gameScoreText.text = manager.scoreText.text + " \n You can save lifes  !";
            }

            else if (manager.score <= 300 && manager.score > 150)
            {
                gameScoreText.text = manager.scoreText.text + " \n You are close to the cure !";
            }

            else if (manager.score <= 150 && manager.score > 50)
            {
                gameScoreText.text = manager.scoreText.text + " \n Well, it can be better !";
            }

            else if (manager.score <= 50)
            {
                gameScoreText.text = manager.scoreText.text + " \n You still have much to learn !";
            }
        }




        

        

    }
    
    public void OpenAlud(string Urlname)
    {
        Application.OpenURL(Urlname);
    }
}
