using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

/* This script displays the score details at the end of the game. */
public class EndGame : MonoBehaviour
{

    [SerializeField] TMP_Text totalScoreText;
    [SerializeField] TMP_Text resultText;
    [SerializeField] TMP_Text resultTitle;
    [SerializeField] TMP_Text gameScoreText;

    private string finalTimeText;
    private GameManager manager;
    private GameObject welcomePanel;
    private GridScript grid;
    public TimeCount timeCount;
    private string[] unitName = new string[6] { "Engineering applied to orthopaedics as an example of the use of biomechanics biomaterials ", "Biosensors and medical devices based on electronic principles", "Surgical Instruments", "Artificial Intelligence and Health", "Anatomy and Physiology", "Genetics and genomics" };
    private bool resultShowed = false;
    private int numberGoodUnit = 0;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //if (welcomePanel.activeSelf == false)
        //{
        //    timeCount = GameObject.Find("Time Text").GetComponent<TimeCount>();

        //}
        //timeCount = GameObject.Find("Time Text").GetComponent<TimeCount>();
    }

    // Update is called once per frame
    void Update()
    {
       // finalTimeText = "Your time : " + timeCount.timerText.text[7..];
        //finalTimeText = "Your time : " + (15 - int.Parse(timeCount.timerText.text.Substring(6, 3))).ToString() + ":" + (60 - int.Parse(timeCount.timerText.text[10..])).ToString();
        finalTimeText = "Your time : " + timeCount.timerText.text;
        //totalScoreText.text = "Your score : " + manager.score.ToString() + "\n" + finalTimeText + "\n" + "Your mark : " + manager.playerMark + "/20";
        totalScoreText.text = "Your score line by line :  \n Unit 2 : " + manager.unit[0] + " / 5 \n Unit 3 : " + manager.unit[1] + " / 5 \n Unit 4 : " + manager.unit[2] + " / 5 \n Unit 5 : " + manager.unit[3] + " / 5 \n Unit 6 : " + manager.unit[4] + " / 5 \n Unit 7 : " + manager.unit[5] + " / 5";
        ScoreSentences();
        Result();

    }


    public void Result()
    {
        if(!resultShowed)
        {
            if (manager.isMaze == true)
            {
               // based on number of green box, add different resultText.text ...
               if (manager.nbClosedDoor >= (grid._width * grid._height) -10)
               {
                    resultText.text = " Well done, you find the cure to save everyone !";
               } 
               else if (manager.nbClosedDoor <= (grid._width * grid._height) / 4)
               {
                    resultText.text = " You made some mistakes but don't lose hope, you can try again !"; 
               }
               else
               {
                    resultText.text = " You are close, your effort allow you to save people !";
                }
            }
            else 
            {
                for (int i = 0; i < manager.unit.Length; i++)
                {
                    if (manager.unit[i] >= 3)
                    {
                        //resultText.text = resultText.text + " \n • " + unitName[i];
                        numberGoodUnit++;
                    }
                }

                if (numberGoodUnit == 6)
                {
                    AdvancedResult();
                }
                else
                {
                    BasicResult();
                }
                timeCount.timerText.gameObject.SetActive(false);
                resultShowed = true;
            }
            
        }

    }

    void BasicResult()
    {
        resultText.text = "Unit 1. Health 4.0\r\nUnit 2. Engineering applied to orthopaedics as an example of the use of biomechanics biomaterials \r\nUnit 4. Surgical Instruments\r\nUnit 6. Anatomy and Physiology\r\n";
    }

    void AdvancedResult()
    {
        resultText.text = "Unit 1. Health 4.0\r\nUnit 2. Engineering applied to orthopaedics as an example of the use of biomechanics biomaterials \r\nUnit 3. Biosensors and medical devices based on electronic principles\r\nUnit 4. Surgical Instruments\r\nUnit 5. Artificial Intelligence and Health\r\nUnit 6. Anatomy and Physiology\r\nUnit 7. Genetics and genomics\r\n";
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
