using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.IO;

/* Script that manages the game */
public class GameManager : MonoBehaviour
{
    public GameObject TitleScreen;
    public GameObject QuestionScreen;
    public GameObject[] Choices;
    public GameObject ScorePanel;
    public GameObject ScoreText;
    public GameObject CloseButton;
    public GameObject RulesButton;
    public GameObject Joker;
    public GameObject DamageScreen;
    public GameObject HeartImage;

    [Header("Damage Overlay---------")]
    public Image overlay; // our DamageOverlay gameobject (coming soon)
    public float duration; // how long the image stays fully opaque
    public float fadespeed; // how quickly the image will fade

    private float durationTimer; // timer to check against the duration


    public TextMeshProUGUI QuestionText;
    public Button QuestionButton;
    public List<string> QnA;
    public Tile tile;
    public EndGame endGame;
    public GridScript grid;
    public SelectedButton selectedButton;
    public Toggle toggleChoice1, toggleChoice2,toggleChoice3, toggleChoice4;
    //private Color selectedColor, normalColor;

    [System.NonSerialized]
    public int tileX,tileY;

    private string CorrectAnswer;
    private GameObject SelectedButton, SelectedButton2;
    private bool CorrectChoice = false;
    private bool isExitRoom = false;
    public  bool questionPopped;
    public  int score;
    private int RandomIndex;
    private int nbWrongAnswers = 0;
    private int nbCorrectAnswers = 0;
    private int nbLines = 0;
    private int nbDisplayedQuestions = 0;
    private int nbJokers = 3;
    private int maxScore = 450;
    public float playerMark;

    [SerializeField] TMP_Text notificationText;
    [SerializeField] TMP_Text timeText;
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text jokerText;

    void Start()
    {
        //var QuizFile = Resources.Load<TextAsset>("/QnA_Files/QnA");
        var QuizFile = Path.Combine(Application.streamingAssetsPath,"QnA_Files/QnA_biomechanics.csv");
        string FilePath = QuizFile;

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

        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0); 
    
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


        if (overlay.color.a > 0)
        {
            durationTimer += Time.deltaTime;
            if(durationTimer > duration)
            {
                //fade the image
                float tempAlpha = overlay.color.a;
                tempAlpha -= Time.deltaTime * fadespeed;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, tempAlpha);
            }
        }

        if (ScorePanel.activeSelf == true)
        {
            HeartImage.gameObject.SetActive(false);
        }
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
                if (nbDisplayedQuestions >= nbLines -1) // In the current version of the game, this would never happened. However, the exception is still managed. If the number of questions is fewer than the number of rooms' maze and the player has managed to get out. He failed and the game is over.
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
        // If we have two choices of correct answers (true or false questions)
        if (QnA[RandomIndex + 1].Split(',').Length == 2)
        {
            Debug.Log("True or false question");
            toggleChoice2.gameObject.SetActive(false);
            toggleChoice4.gameObject.SetActive(false);
            // Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[0];
            Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[0];
            Choices[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[1];



        }



        // If we have three choices of correct answers
        if (QnA[RandomIndex + 1].Split(',').Length == 3)
        {
            Debug.Log("three answers question");
            toggleChoice4.gameObject.SetActive(false);
            for (int j = 0; j < 3; j++)
            {
                //Choices[j].transform.GetChild(1).GetComponent<Text>().text = (QnA[RandomIndex + 1].Split(','))[j];
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[j];
            }
        }



        //If we have four choices of correct answers (a real MCQ)
        if (QnA[RandomIndex + 1].Split(',').Length > 3)
        {
            for (int j = 0; j < 4; j++)
            {
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[j];
            }
        }

        CorrectAnswer = QnA[RandomIndex + 2];
    }

    // Check if the player cliked on the correct answer
    public void IsCorrect()
   {
        //CheckButton(Choices);
        CheckToggle();
   }

   public void CheckToggle()
   {
        // If we only have one correct answer
        if (!CorrectAnswer.Contains(","))
        {
            if (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == CorrectAnswer)
            {
                if (!toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn && toggleChoice1.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        isExitRoom = true;
                    }
                }
                else
                {
                    CorrectChoice = false;
                    //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                }
            }

            else if (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == CorrectAnswer)
            {
                if (!toggleChoice1.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn && toggleChoice2.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        isExitRoom = true;
                    }
                }
                else
                {
                    CorrectChoice = false;
                    //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                }
            }
            else if (toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == CorrectAnswer)
            {
                if (!toggleChoice2.isOn && !toggleChoice1.isOn && !toggleChoice4.isOn && toggleChoice3.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        isExitRoom = true;
                    }
                }
                else
                {
                    CorrectChoice = false;
                    //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                }
            }
            else if (toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == CorrectAnswer)
            {
                if (!toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice1.isOn && toggleChoice4.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        isExitRoom = true;
                    }
                }
                else
                {
                    CorrectChoice = false;
                   // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                }
            }
        }

        // If we have multiple correct answers
        if (CorrectAnswer.Contains(","))
        {
            string[] Answers = CorrectAnswer.Split(",");
            int nbAnswers = Answers.Length;

            // If we have two correct answers
            if (nbAnswers == 2) 
            {
                string correctAnswer1 = Answers[0]; 
                string correctAnswer2 = Answers[1]; 
                Debug.Log(correctAnswer1 + "," + correctAnswer2);

                // If toggle 1 and 2 are holding the correct answers 
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                     (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice1.isOn && toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 1 and 3 are holding the correct answers
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice1.isOn && toggleChoice3.isOn && !toggleChoice2.isOn && !toggleChoice4.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                        //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 1 and 4 are holding the correct answers
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice1.isOn && toggleChoice4.isOn && !toggleChoice3.isOn && !toggleChoice2.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 2 and 3 are holding the correct answers
                if ((toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                     (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice2.isOn && toggleChoice3.isOn && !toggleChoice1.isOn && !toggleChoice4.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                        //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 2 and 4 are holding the correct answers
                if ((toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice2.isOn && toggleChoice4.isOn && !toggleChoice1.isOn && !toggleChoice3.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                        //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 3 and 4 are holding the correct answers
                if ((toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2) ||
                    (toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1))
                {
                    if (toggleChoice3.isOn && toggleChoice4.isOn && !toggleChoice1.isOn && !toggleChoice2.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }
            }

            // If we have three correct answers
            if (nbAnswers == 3)
            {
                string correctAnswer1 = Answers[0];
                string correctAnswer2 = Answers[1];
                string correctAnswer3 = Answers[2];
                Debug.Log(correctAnswer1 + "," + correctAnswer2 + "," + correctAnswer3);

                // If toggle 1, 2, and 3 are holding the correct answers 
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)
                    )
                {
                    if (toggleChoice1.isOn && toggleChoice2.isOn && toggleChoice3.isOn && !toggleChoice4.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 1, 2, and 4 are holding the correct answers 
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)
                    )
                {
                    if (toggleChoice1.isOn && toggleChoice2.isOn && toggleChoice4.isOn && !toggleChoice3.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 1, 3, and 4 are holding the correct answers 
                if ((toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)||
                    (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)
                )
                {
                    if (toggleChoice1.isOn && toggleChoice3.isOn && toggleChoice4.isOn && !toggleChoice2.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                       // toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }

                // If toggle 2, 3, and 4 are holding the correct answers 
                if ((toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3)||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1)||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)||
                    (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer1 && toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer3 && toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer2)
                )
                {
                    if (toggleChoice1.isOn && toggleChoice3.isOn && toggleChoice4.isOn && !toggleChoice2.isOn)
                    {
                        CorrectChoice = true;
                        // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                        if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                        {
                            //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                        }
                        if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                        {
                            isExitRoom = true;
                        }
                    }
                    else
                    {
                        CorrectChoice = false;
                      //  toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                    }
                }
            }

        }
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
                if (Choices[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text == CorrectAnswer) //Choices[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text == CorrectAnswer
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    //if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)  
                    //{
                    //    grid._listTiles[tileX, tileY].tag = "OpenedDoor";         
                    //}
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
            toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
            toggleChoice1.gameObject.SetActive(true);
            toggleChoice2.gameObject.SetActive(true);
            toggleChoice3.gameObject.SetActive(true);
            toggleChoice4.gameObject.SetActive(true);
            QuestionScreen.SetActive(false);
            questionPopped = false;
            grid._listTiles[tileX, tileY].tag = "OpenedDoor";
            StartCoroutine(ShowMessage("Well done! You can move to the green tile.", 4));
            QuestionButton.gameObject.SetActive(true);
            score += 5;
            QnA.Remove(QnA[RandomIndex]); // remove the question from the list so that it doesn't show up again
           
            //Score Bonuses 
            if (nbCorrectAnswers == 2)
            {
                score += 5;
                StartCoroutine(ShowMessage("Keep it up! Two good answers in a row. +5 to your score.", 6));
                Debug.Log("1er bonus: +5");
            }
            if (nbCorrectAnswers == 3)
            {
                score += 10;
                StartCoroutine(ShowMessage("Excellent! Three good answers in a row. +10 to your score.", 6));
                Debug.Log("2e bonus: +10");
            }
            if (nbCorrectAnswers == 4)
            {
                score += 15;
                StartCoroutine(ShowMessage("You are smashing it! +15 to your score.", 5));
                Debug.Log("3e bonus: +15");
            }
            if (nbCorrectAnswers >= 5)
            {
                score += 20;
                StartCoroutine(ShowMessage("Spectacular! +20 to your score.", 5));
                Debug.Log("4e bonus: +20");
            }
        } 
        else if (CorrectChoice && isExitRoom == true)
        {
            QuestionScreen.SetActive(false);
            ScorePanel.SetActive(true);
            ScoreText.SetActive(false);            
            timeText.gameObject.SetActive(true);
            Joker.SetActive(false);
            RulesButton.SetActive(false);
            isExitRoom = false;
            HeartImage.gameObject.SetActive(false);
            
        }
        else if ((!toggleChoice1.isOn && !toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn) ||
                (!toggleChoice1.isOn && !toggleChoice2.isOn && !toggleChoice3.isOn && Choices.Length == 3) ||
                (!toggleChoice1.isOn && !toggleChoice3.isOn && Choices.Length == 2) )
        {
            StartCoroutine(ShowMessage("You haven't selected any answers. If you're struggling and you have jokers, you can skip the question.", 6));
        }
        else
        {
            toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
            nbCorrectAnswers = 0;
            nbWrongAnswers++;
            // Score Penalties 
            if(nbWrongAnswers == 1)
            {
                StartCoroutine(ShowMessage("You gave the wrong answer. -5 to your score. Try again.", 6));
                score -= 5;
                Debug.Log("1er malus: -5");
            }
            if (nbWrongAnswers == 2)
            {
                StartCoroutine(ShowMessage("You chose the wrong answer a second time. -10 to your score. Try again.", 6));
                score -= 10;
                Debug.Log("2e malus: -15");
                //durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            if (nbWrongAnswers == 3)
            {
                StartCoroutine(ShowMessage("You chose the wrong answer a third time. -15 to your score. Try again.", 6));
                score -= 20;
                Debug.Log("3e malus: -15");
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            if (nbWrongAnswers == 4)
            {
                StartCoroutine(ShowMessage("You chose the wrong answer a fourth time. -20 to your score. Try again.", 6));
                score -= 20;
                Debug.Log("4e malus: -20");
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            else if(nbWrongAnswers > 4)
            {
                Debug.Log("Wrong answer given again.");
                StartCoroutine(ShowMessage("Wrong again! Stay focused and think harder.", 6));
                score -= 20;
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            } 
        }
        CorrectChoice = false;
        TransformScore();
        Debug.Log(playerMark);
        tile.questionpoped = false;
        Debug.Log("Check les réponses");
    } 

    // Close the panel question
    public void ClosePanel()
    {
        if (nbJokers != 0)
        {
            nbJokers--;
            QuestionScreen.SetActive(false);
            questionPopped = false;
            
                grid._listTiles[tileX, tileY].tag = "Obstacle";
 
            StartCoroutine(ShowMessage("Careful, by skipping the question, you've used up 1 joker. You only have " + nbJokers.ToString() + " left now.", 8));
        }

        jokerText.text = "Joker : " + nbJokers.ToString() + "/3";

        if (nbJokers == 0)
        {
            CloseButton.SetActive(false);
        }
        tile.questionpoped = false;
        Debug.Log("joker utilisé");

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

    // Transform the score to a mark
    public void TransformScore()
    {
        if (score <= 0)
        {
            playerMark = 0;
        }
        else if (score > 0 && score < 115)
        {
            playerMark = score / 10f;
        }
        else if (score == 115)
        {
            playerMark = 11;
        }
        else if (score > 115 && score < 140)
        {
            playerMark = 11.5f;
        }
        else if (score >= 140 && score <= 145)
        {
            playerMark = 12;
        }
        else if (score > 145 && score < 170)
        {
            playerMark = 12.5f;
        }
        else if (score == 170)
        {
            playerMark = 13;
        }
        else if (score > 170 && score < 195)
        {
            playerMark = 13.5f;
        }
        else if (score >= 195 && score <= 200)
        {
            playerMark = 14;
        }
        else if (score > 200 && score < 225)
        {
            playerMark = 14.5f;
        }
        else if (score >= 225 && score <= 230)
        {
            playerMark = 15;
        }
        else if (score > 230 && score < 275)
        {
            playerMark = 15.5f;
        }
        else if (score >= 275 && score <= 280)
        {
            playerMark = 16;
        }
        else if (score > 280 && score < 335)
        {
            playerMark = 16.5f;
        }
        else if (score >= 335 && score <= 340)
        {
            playerMark = 17;
        }
        else if (score > 340 && score < 390)
        {
            playerMark = 17.5f;
        }
        else if (score >= 390 && score <= 395)
        {
            playerMark = 18;
        }
        else if (score > 395 && score < 420)
        {
            playerMark = 18.5f;
        }
        else if (score >= 420 && score <= 425)
        {
            playerMark = 19;
        }
        else if (score > 425 && score < maxScore)
        {
            playerMark = 19.5f;
        }
        else if (score >= maxScore)
        {
            playerMark = 20;
        }

        Debug.Log("Your mark is : " + playerMark + "/20");
    }
}
