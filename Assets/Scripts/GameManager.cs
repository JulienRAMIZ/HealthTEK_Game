using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/* Script that manages the game */
public class GameManager : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject QuestionScreen;
    public GameObject[] Choices;
    public GameObject ScorePanel;
    public GameObject ScoreText;
    public TextMeshProUGUI QuestionText;
    public Button QuestionButton;
    public List<string> QnA;
    public Tile tile;
    public EndGame endGame;
    public GridScript grid;
   
    [System.NonSerialized]
    public int tileX,tileY;

    private string CorrectAnswer;
    private GameObject SelectedButton;
    private bool CorrectChoice = false;
    private bool isExitRoom = false;
    public bool questionPopped;
    public int score;
    private int RandomIndex;
    private int nbWrongAnswers = 0;
    private int nbCorrectAnswers = 0;
    private int nbLines = 0;
    private int nbDisplayedQuestions = 0;

    [SerializeField] TMP_Text notificationText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text scoreText;

    void Start()
    {
        string FilePath = Application.dataPath + "/QnA_Files/QnA.csv";
        Debug.Log(FilePath);

        QuestionScreen.SetActive(false);
        ScorePanel.SetActive(false);
        timeText.gameObject.SetActive(false);

        //Open the questions and answers file (csv file), retrieve the values and add them in a list
        var reader = new StreamReader(File.OpenRead(FilePath));
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(';');
            QnA.Add(values[0]);
            QnA.Add(values[1]);
            QnA.Add(values[2]);
            nbLines++; // We use nbLines here to get the number of questions. nbLines corresponds to the number of lines in the csv file. Since each line holds a question, nbLines can count as the number of questions.
        }
        QnA.RemoveRange(0, 3); //remove the first three values of the list (Question, Possible_Answer, Correct_Answer)
    }

    /* Possible evolution of the game. Here we set the difficulty.
     * Start the game, remove title screen, reset timer, and adjust the questions based on difficulty button clicked.
       public void StartGame(int difficulty)
      {
        TitleScreen.SetActive(false); 
      }
    */

   // Update is called once per frame
   void Update()
   {
        // Score updating
        scoreText.text = "Score : " + score.ToString();
   }

   // Choose a question randomly and displays it along with its possible answers
   public void PopUpQuestion() 
   {
        QuestionScreen.SetActive(true);
        QuestionButton.gameObject.SetActive(false);
        RandomIndex = UnityEngine.Random.Range(0, QnA.Count);

        if (QnA[RandomIndex].EndsWith('?'))
        {
            QuestionText.text = QnA[RandomIndex];
            nbDisplayedQuestions++;
        }
        else
        {
            while (!QnA[RandomIndex].EndsWith('?'))
            {
                if (nbDisplayedQuestions >= nbLines -1) // In the current version of the game, this would never happened. However, the exception is still managed. If the number of question is fewer than the number of rooms' maze and the player has managed to get out. He failed and the game is over.
                {
                    StartCoroutine(ShowMessage("No questions left. The Game is over. You didn't succeed. Your score will be displayed in a few seconds.", 7));
                    ScorePanel.SetActive(true);
                    break;
                }
                RandomIndex = UnityEngine.Random.Range(0, QnA.Count);
                QuestionText.text = QnA[RandomIndex];
            }
            nbDisplayedQuestions++;
        }
        SetAnswers();
        questionPopped = true;
   }

   // Set the four possible answers to the choice buttons on Unity and assign the correct answer
   public void SetAnswers() 
   {
        for (int j = 0; j < 4; j++)
        {
            Choices[j].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[j];
        }
        CorrectAnswer = QnA[RandomIndex + 2];
   }

   // Check if the player cliked on the correct answer
   public void IsCorrect()
    {
        CheckButton(Choices);
    }
   
   // Check if the button selected holds the correct answer
   public void CheckButton(GameObject[] Choices)
   {
        SelectedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log(grid._listTiles[tileX, tileY].tag);
        for (int i = 0; i < Choices.Length; i++)
        {
            if (SelectedButton == Choices[i])
            {
                if (Choices[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == CorrectAnswer)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)  
                    {
                        grid._listTiles[tileX, tileY].tag = "OpenedDoor";         
                    }
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        isExitRoom = true;
                    }
                    break;
                }
                else
                {
                    CorrectChoice = false;
                }
            }
        }
   }
   
   // Coroutine for the message display during the game
   public IEnumerator ShowMessage (string message,float Delay)
   {
        bool textEnabled = true;
        while (textEnabled)
        {
            notificationText.text = message;
            yield return new WaitForSeconds(Delay);
            notificationText.text = "";
            textEnabled = false;
        }  
   }
   
   // Update the score depending on whether or not the correct answer has been given and the display the message that goes with it
   public void CheckCorrect()
   {
        if (CorrectChoice && isExitRoom == false)
        {
            nbCorrectAnswers++;
            nbWrongAnswers = 0;
            QuestionScreen.SetActive(false);
            questionPopped = false;
            StartCoroutine(ShowMessage("Well done! You can move to the green tile.", 4));
            QuestionButton.gameObject.SetActive(true);
            score += 5;
            QnA.Remove(QnA[RandomIndex]); // remove the question from the list so that it doesn't show up again
           
            //Score Bonuses 
            if (nbCorrectAnswers == 2)
            {
                score += 5;
                StartCoroutine(ShowMessage("Keep it up! Two good answers in a row. +5 to your score.", 6));
            }
            if (nbCorrectAnswers == 3)
            {
                score += 10;
                StartCoroutine(ShowMessage("Excellent! Three good answers in a row. +10 to your score.", 6));
            }
            if (nbCorrectAnswers >= 4)
            {
                score += 15;
                StartCoroutine(ShowMessage("You are smashing it! +15 to your score.", 5));
            }
        } 
        else if (CorrectChoice && isExitRoom == true)
        {
            QuestionScreen.SetActive(false);
            ScorePanel.SetActive(true);
            ScoreText.SetActive(false);            
            timeText.gameObject.SetActive(true);
            isExitRoom = false; 
        }
        else
        {
            nbCorrectAnswers = 0;
            nbWrongAnswers++;

            // Score Penalties 
            if(nbWrongAnswers == 1)
            {
                Debug.Log("Wrong answer given one time.");
                StartCoroutine(ShowMessage("You gave the wrong answer. -5 to your score. Try again.", 6));
                score -= 5;
            }
            if (nbWrongAnswers == 2)
            {
                Debug.Log("Wrong answer given two times.");
                StartCoroutine(ShowMessage("You chose the wrong answer a second time. -10 to your score. Try again.", 6));
                score -= 10;
            }
            if (nbWrongAnswers == 3)
            {
                Debug.Log("Wrong answer given three times.");
                StartCoroutine(ShowMessage("You chose the wrong answer a third time. -20 to your score. Try again.", 6));
                score -= 20;
            }
            else if(nbWrongAnswers > 3)
            {
                Debug.Log("Wrong answer given again.");
                StartCoroutine(ShowMessage("Wrong again! Stay focused. -40 to your score. Think harder and try again.", 6));
                score -= 40;
            } 
        }   
   } 
   
    //Restart game by reloading the scene
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Quit the game
    public void QuitGame()
    {
        Application.Quit();
    }
}
