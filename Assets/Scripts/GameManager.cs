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
using Nobi.UiRoundedCorners;
using System.Linq;
using UnityEditor;
using Unity.VisualScripting;
using static Unity.VisualScripting.Member;

/* Script that manages the game */
public class GameManager : MonoBehaviour
{
    public string lienFichier;
    public GameObject TitleScreen;
    public GameObject QuestionScreen;
    public GameObject[] Choices;
    public GameObject ScorePanel;  
    public GameObject ToggleGroup;  
    public GameObject ScoreText;
    public GameObject CloseButton;
    public GameObject RulesButton;
    public GameObject tryAgainButton;
    public GameObject Joker;
    public GameObject DamageScreen;
    public GameObject HeartImage;

    [Header("Damage Overlay---------")]
    public Image overlay; // our DamageOverlay gameobject (coming soon)
    public float duration; // how long the image stays fully opaque
    public float fadespeed; // how quickly the image will fade

    private float durationTimer; // timer to check against the duration


    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI NumberAnswerText;
    public Button QuestionButton;
    public List<string> QnA;
    public Tile tile;
    public PlayerController player;
    public EndGame endGame;
    public GridScript grid;
    public TimeCount timeCount;
    public SelectedButton selectedButton;
    public Toggle toggleChoice1, toggleChoice2, toggleChoice3, toggleChoice4;
    public Toggle[] toggleChoice; 
    public GameObject background1 , background2, background3, background4;
    private ImageWithRoundedCorners image1, image2, image3, image4;
    public Sprite square, circle;
    //private Color selectedColor, normalColor;

    [System.NonSerialized]
    public int tileX,tileY;

    private string[] correctAnswer = new string[4];
    private string[] filledAnswer = new string[4];
    private string[] filledWithEmpty = new string[4];
    private string[] answerAndEmpty = new string[4];
    private int nbEmptyAnswer = 0;
    private GameObject SelectedButton, SelectedButton2;
    private GameObject[] closedRoom;
    private bool CorrectChoice = false;
    private bool isExitRoom = false;
    private bool endMaze = false;
    public  bool questionPopped;
    public  bool isNotification = false;
    public  int score;
    public int[] unit = new int[6];
    public int unit2;
    public int unit3;
    public int unit4;
    public int unit5;
    public int unit6;
    public int unit7;
    private int QuestionIndex = 0;
    private int nbWrongAnswers = 0;
    private int nbCorrectAnswers = 0;
    private int nbLines = 0;
    private int nbDisplayedQuestions = 0;
    private int nbJokers = 3;
    private int maxScore = 450;
    private int randomIndex = 0;
    private bool isMaze = false;
    public float playerMark;

    [SerializeField] TMP_Text notificationText;
    [SerializeField] TMP_Text identificationText;
    [SerializeField] TMP_Text timeText;
    public TMP_Text scoreText;
    [SerializeField] TMP_Text jokerText;

    void Start()
    {
        //var QuizFile = Resources.Load<TextAsset>("/QnA_Files/QnA");
        //var QuizFile = Path.Combine(Application.streamingAssetsPath,"QnA_Files/QnA_unit1.csv");
        var QuizFile = Path.Combine(Application.streamingAssetsPath, "QnA_Files",lienFichier);
        string FilePath = QuizFile;

        Debug.Log(FilePath);

        QuestionScreen.SetActive(false);
        ScorePanel.SetActive(false);
        timeText.gameObject.SetActive(false);

        ////Open the questions and answers file (csv file), retrieve the values and add them in a list
        //var reader = new StreamReader(File.OpenRead(FilePath));
  

        //Open the questions and answers file (csv file), retrieve the values and add them in a list
        var reader = new StreamReader(File.OpenRead(FilePath));
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(';');
            QnA.Add(values[0]);
            QnA.Add(values[1]);
            QnA.Add(values[2]);
            QnA.Add(values[3]);
            QnA.Add(values[4]);
            QnA.Add(values[5]);
            QnA.Add(values[6]);
            QnA.Add(values[7]);
            QnA.Add(values[8]);
            //QnA.Add(values[9]);
            //QnA.Add(values[10]);
            nbLines++; // We use nbLines here to get the number of questions. nbLines corresponds to the number of lines in the csv file. Since each line holds a question, nbLines can count as the number of questions.
        }
        //On ne supprime plus la ligne de titre car elle n'existe plus dans le fichier
        // Au cas où c'était : question;answer1;answer2;answer3;answer4;correct1;correct2;correct3;correct4;pos x;pos y
        //QnA.RemoveRange(0, 11); //remove the first three values of the list (Question, Possible_Answer, Correct_Answer)

        overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0);

        //Debug.Log("Nombre de lignes : " + nbLines); 
        //CorrectAnswer = QnA[RandomIndex + 2];

        // create a function for adapt file's answers
        Debug.Log("On regarde le qna count" + QnA.Count);



        image1 = background1.GetComponent<ImageWithRoundedCorners>();
        image2 = background2.GetComponent<ImageWithRoundedCorners>();
        image3 = background3.GetComponent<ImageWithRoundedCorners>();
        image4 = background4.GetComponent<ImageWithRoundedCorners>();
        OnlyOneToggle();
    }


   // Update is called once per frame
   void Update()
   {
        // Score updating
        //scoreText.text = "Score : " + score.ToString();
        scoreText.gameObject.SetActive(false);

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


        closedRoom = GameObject.FindGameObjectsWithTag("ClosedDoor");


        if (closedRoom.Length <= 0)
        {
            EndMaze();

        }

    }

    public void EndMaze()
    {
        if (!endMaze)
        {
            CountScore();
            QuestionScreen.SetActive(false);
            ScorePanel.SetActive(true);
            ScoreText.SetActive(false);
            timeText.gameObject.SetActive(true);
            RulesButton.SetActive(false);
            tryAgainButton.SetActive(false);
            isExitRoom = false;
            HeartImage.gameObject.SetActive(false);
            endMaze = true;
        }
    }

    public void CountScore()
    {
        //Debug.Log("début");
        for (int i = 0; i < grid._height; i++)
        {
            //Debug.Log("On est dans les i : " + i );
            for (int j = 0; j < grid._width; j++)
            {
                //Debug.Log("On est dans les j : " + j);
                if (grid._listTiles[j, i].tag == "OpenedDoor")
                {
                    //Debug.Log("Cette room est juste  :  x " + j + "y " + i + "  " + grid._listTiles[j, i].tag);
                    unit[i]++;
                }
            }
        }
    }

    public void QuestionTile()
    {

        //QuestionIndex = (tileX + (tileY * 5)) * 11;
        if (lienFichier == "questions_michel_NS.csv")
        {
            isMaze = true;

            randomIndex = UnityEngine.Random.Range(0, QnA.Count / 9);
            QuestionIndex = randomIndex * 9;
        }
        else
        {
            QuestionIndex = (tileX + (tileY * grid._width)) * 9;
        }




 
        PopUpQuestion();

    }

    public void AdaptFileAnswers()
    {

        // create a function to adapt file's answers
        for (int i = 0; i <= 3; i++)
        {
            //for (int j = 0; j <= 3; j++)
            //{

            answerAndEmpty[i] = QnA[QuestionIndex + 5 + i];
            filledWithEmpty[i] = QnA[QuestionIndex + 1 + i];

            if (answerAndEmpty[i] == "EMPTY")
            {
                nbEmptyAnswer++;

            }
            else
            {
                //for (int j = 0; j < 4; j++)
                //{
                answerAndEmpty[i] = filledWithEmpty[i];
                //}


            }
        }
            //}
            var total = 0;
            total = answerAndEmpty.Count(c => c == "EMPTY");
            //Debug.Log(total);


            // Permet de repérer les éléments qui ont EMPTY et de les supprimer de l'array
            var Empty = new HashSet<string> { "EMPTY" };
            //var test = answerAndEmpty.ToHashSet());
            var test = System.Linq.Enumerable.ToHashSet(answerAndEmpty);


        filledAnswer = filledWithEmpty.Except(new List<string> { string.Empty }).ToArray();
        test.ExceptWith(Empty);
            correctAnswer = test.ToArray();

            //Debug.Log("voici correct answer : ");
            for (int k = 0; k < correctAnswer.Length - 1; k++) 
            { 
                //Debug.Log(correctAnswer[k]);   
            }

    }

   // Choose a question randomly and displays it along with its possible answers
   public void PopUpQuestion() 
   {
        //Debug.Log("TUTE TUTE FILS DE PUTE");
        QuestionScreen.SetActive(true);
        QuestionButton.gameObject.SetActive(false);
        //RandomIndex = UnityEngine.Random.Range(0, QnA.Count);
        Transform[] backgroundM = new Transform[4];
        Transform[] backgroundS = new Transform[4];
        backgroundM[0] = toggleChoice1.transform.Find("BackgroundM");
        backgroundM[1] = toggleChoice2.transform.Find("BackgroundM");
        backgroundM[2] = toggleChoice3.transform.Find("BackgroundM");
        backgroundM[3] = toggleChoice4.transform.Find("BackgroundM");
        backgroundS[0] = toggleChoice1.transform.Find("BackgroundM");
        backgroundS[1] = toggleChoice2.transform.Find("BackgroundM");
        backgroundS[2] = toggleChoice3.transform.Find("BackgroundM");
        backgroundS[3] = toggleChoice4.transform.Find("BackgroundM");
        


        

        if (QnA[QuestionIndex].EndsWith('?'))
        {
            QuestionText.text = QnA[QuestionIndex];
            nbDisplayedQuestions++;
            //Debug.Log("Index question : " + QuestionIndex);
        }
        else
        {
            if (!QnA[QuestionIndex].EndsWith('?'))
            {
                if (nbDisplayedQuestions >= nbLines - 1) // In the current version of the game, this would never happened. However, the exception is still managed. If the number of questions is fewer than the number of rooms' maze and the player has managed to get out. He failed and the game is over.
                {
                    StartCoroutine(ShowMessage("No questions left. The Game is over. You didn't succeed. Your score will be displayed in a few seconds.", 7));
                    ScorePanel.SetActive(true);

                }
            }
            //nbDisplayedQuestions++;
            Debug.Log("Ok c'est la merde ");
            QuestionTile();
        }
        //SetAnswers();
        newSetAnswer();
        questionPopped = true;
        nbEmptyAnswer = 0;
    }

    // Set the four possible answers to the choice buttons on Unity and assign the correct answer

    public void newSetAnswer()
    {
        AdaptFileAnswers();

        if (nbEmptyAnswer <= 2)
        {
            //NumberAnswerText.text = "Chose many ! ";
            QuestionText.text = QuestionText.text + "  Chose many ! ";
            //background1.GetComponent<Image>().sprite = square;
            //background2.GetComponent<Image>().sprite = square;
            //background3.GetComponent<Image>().sprite = square;
            //background4.GetComponent<Image>().sprite = square;
            background1.GetComponent<Image>().sprite = circle;
            background2.GetComponent<Image>().sprite = circle;
            background3.GetComponent<Image>().sprite = circle;
            background4.GetComponent<Image>().sprite = circle;
            ToggleGroup.SetActive(false);

        }

        if (nbEmptyAnswer == 3)
        {
            //for (int i = 0; i < 4; i++)
            //{
            //    backgroundM[i].gameObject.SetActive(false);
            //    backgroundS[i].gameObject.SetActive(true);
            //    Debug.Log("Là c'est les ronds");

            //}
            background1.GetComponent<Image>().sprite = circle;
            background2.GetComponent<Image>().sprite = circle;
            background3.GetComponent<Image>().sprite = circle;
            background4.GetComponent<Image>().sprite = circle;

            QuestionText.text = QuestionText.text + "  Chose one ! ";

            ToggleGroup.SetActive(true);

        }

        if (filledAnswer.Length == 2)
        {
            //Debug.Log("True or false question");
            toggleChoice1.gameObject.SetActive(true);
            toggleChoice3.gameObject.SetActive(true);
            toggleChoice2.gameObject.SetActive(false);
            toggleChoice4.gameObject.SetActive(false);
            // Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[0];
            Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[0];
            Choices[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[1];
        }

        // If we have three choices of correct answers
        if (filledAnswer.Length == 3)
        {
            //Debug.Log("three answers question");
            toggleChoice4.gameObject.SetActive(false);
            for (int j = 0; j < 3; j++)
            {
                //Choices[j].transform.GetChild(1).GetComponent<Text>().text = (QnA[RandomIndex + 1].Split(','))[j];
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[j];
                toggleChoice[j].gameObject.SetActive(true);
            }
            toggleChoice4.gameObject.SetActive(false);
        }



        //If we have four choices of correct answers (a real MCQ)
        if (filledAnswer.Length > 3)
        {
            for (int j = 0; j < 4; j++)
            {
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[j];
                toggleChoice[j].gameObject.SetActive(true);
            }
        }
        //QuestionIndex = QuestionIndex + 11;
    }

    public void SetAnswers()
    {
        AdaptFileAnswers();
        // If we have two choices of correct answers (true or false questions)
        if (QnA[QuestionIndex + 1].Split('|').Length == 2)
        {
            Debug.Log("True or false question");
            toggleChoice2.gameObject.SetActive(false);
            toggleChoice4.gameObject.SetActive(false);
            // Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[0];
            Choices[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[0];
            Choices[2].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[1];



        }



        // If we have three choices of correct answers
        if (QnA[QuestionIndex + 1].Split('|').Length == 3)
        {
            Debug.Log("three answers question");
            toggleChoice4.gameObject.SetActive(false);
            for (int j = 0; j < 3; j++)
            {
                //Choices[j].transform.GetChild(1).GetComponent<Text>().text = (QnA[RandomIndex + 1].Split(','))[j];
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[j];
            }
        }



        //If we have four choices of correct answers (a real MCQ)
        if (QnA[QuestionIndex + 1].Split('|').Length > 3)
        {
            for (int j = 0; j < 4; j++)
            {
                Choices[j].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = filledAnswer[j];
            }
        }

    }

    // Check if the player cliked on the correct answer

    public void IsCorrect()
   {
        //CheckButton(Choices);
        //OnlyOneToggle();
        CheckToggle();

   }

    public void OnlyOneToggle()
    {

        toggleChoice = new[] { toggleChoice1, toggleChoice2, toggleChoice3, toggleChoice4 }; 
    }
   public void CheckToggle()
   {
        // If we only have one correct answer
        if (correctAnswer.Length == 1)
        {
            

            if (toggleChoice1.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer[0])
            {
                if (!toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn && toggleChoice1.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                }
                else
                {
                    CorrectChoice = false;
                    //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
                }
            }

            else if (toggleChoice2.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer[0])
            {
                if (!toggleChoice1.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn && toggleChoice2.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                        //grid._listTiles[tileX, tileY].tag = "OpenedDoor";
                    }
                }
                else
                {
                    CorrectChoice = false;
                    //toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;                                                                                                                                                                                                                                                                                                         
                }
            }
            else if (toggleChoice3.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer[0])
            {
                if (!toggleChoice2.isOn && !toggleChoice1.isOn && !toggleChoice4.isOn && toggleChoice3.isOn)
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)
                    {
                    }
 
                }
                else
                {
                    CorrectChoice = false;
                }
            }
            else if (toggleChoice4.GetComponentInChildren<TextMeshProUGUI>().text == correctAnswer[0])
            {
                if (!toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice1.isOn && toggleChoice4.isOn)
                {
                    CorrectChoice = true;
                }
                else
                {
                    CorrectChoice = false;
                }
            }
        }

        // If we have multiple correct answers
        if (correctAnswer.Length >= 2)
        {
            //string[] Answers = filledAnswer;
            string[] Answers = correctAnswer;
            int nbAnswers = Answers.Length;

            // If we have two correct answers
            if (nbAnswers == 2) 
            {
                string correctAnswer1 = Answers[0]; 
                string correctAnswer2 = Answers[1]; 
                Debug.Log(correctAnswer1 + "|" + correctAnswer2);

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
                //Debug.Log(correctAnswer1 + "|" + correctAnswer2 + "|" + correctAnswer3);

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
        //Debug.Log(grid._listTiles[tileX, tileY].tag);

        for (int i = 0; i < Choices.Length; i++)
        {
            if (SelectedButton == Choices[i])
            {
                if (Choices[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text == correctAnswer[i]) //Choices[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text == CorrectAnswer
                {
                    CorrectChoice = true;
                    // Get the room from where the question popped and change its tag. The room's position comes from the script Tile via the OnPointerDown() function
                    //if (grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == false)  
                    //{
                    //    grid._listTiles[tileX, tileY].tag = "OpenedDoor";         
                    //}
                    if (tileX == 4 && tileY == 4 /*grid._listTiles[tileX, tileY].CompareTag("ExitRoom") == true*/)
                    {
                        //isExitRoom = true;
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
            notificationText.text = "Click on a box to answer a question.";
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
            StartCoroutine(ShowMessage("Well done! ", 4));
            QuestionButton.gameObject.SetActive(true);
            score += 5;
            //QnA.Remove(QnA[QuestionIndex]); // remove the question from the list so that it doesn't show up again
           
            //Score Bonuses 
            if (nbCorrectAnswers == 2)
            {
                score += 5;
                StartCoroutine(ShowMessage("Keep it up! Two good answers in a row.", 4));
                //Debug.Log("1er bonus: +5");
            }
            if (nbCorrectAnswers == 3)
            {
                score += 10;
                StartCoroutine(ShowMessage("Excellent! Three good answers in a row. ", 4));
                //Debug.Log("2e bonus: +10");
            }
            if (nbCorrectAnswers == 4)
            {
                score += 15;
                StartCoroutine(ShowMessage("You are smashing it! ", 4));
                //Debug.Log("3e bonus: +15");
            }
            if (nbCorrectAnswers >= 5)
            {
                score += 20;
                StartCoroutine(ShowMessage("Spectacular! ", 4));
                //Debug.Log("4e bonus: +20");
            }
            questionPopped = false;
            player.isAnswered = true;
            player.ableMoving = true;
            player.closedDoor = false;
        } 
        //else if (CorrectChoice && isExitRoom == true)
        //else if (closedRoom.Length <= 0)
        //{
        //    QuestionScreen.SetActive(false);
        //    endGame.CountScore();
        //    ScorePanel.SetActive(true);
        //    ScoreText.SetActive(false);            
        //    timeText.gameObject.SetActive(true);
        //    Joker.SetActive(false);
        //    RulesButton.SetActive(false);
        //    isExitRoom = false;
        //    HeartImage.gameObject.SetActive(false);
            
        //}
        else if ((!toggleChoice1.isOn && !toggleChoice2.isOn && !toggleChoice3.isOn && !toggleChoice4.isOn) ||
                (!toggleChoice1.isOn && !toggleChoice2.isOn && !toggleChoice3.isOn && Choices.Length == 3) ||
                (!toggleChoice1.isOn && !toggleChoice3.isOn && Choices.Length == 2) )
        {
            StartCoroutine(ShowMessage("You haven't selected any answers.", 4));
        }
        else
        {
            toggleChoice1.isOn = false; toggleChoice2.isOn = false; toggleChoice3.isOn = false; toggleChoice4.isOn = false;
            nbCorrectAnswers = 0;
            nbWrongAnswers++;
            toggleChoice1.gameObject.SetActive(true);
            toggleChoice2.gameObject.SetActive(true);
            toggleChoice3.gameObject.SetActive(true);
            toggleChoice4.gameObject.SetActive(true);
            QuestionScreen.SetActive(false);
            questionPopped = false;
            grid._listTiles[tileX, tileY].tag = "FailedRoom";
            //StartCoroutine(ShowMessage("You failed, but don't lose hope", 6));
            QuestionButton.gameObject.SetActive(true);
            // Score Penalties 
            if (nbWrongAnswers == 1)
            {
                //StartCoroutine(ShowMessage("You gave the wrong answer. -5 to your score. Try again.", 6));
                StartCoroutine(ShowMessage("You failed, but don't lose hope. ", 4));
                score -= 5;
                //Debug.Log("1er malus: -5");
            }
            if (nbWrongAnswers == 2)
            {
                StartCoroutine(ShowMessage("That wasn't the good choice. ", 4));
                score -= 10;
                //Debug.Log("2e malus: -15");
                //durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            if (nbWrongAnswers == 3)
            {
                StartCoroutine(ShowMessage("Wrong answer, but it's not over  ", 4));
                score -= 20;
                //Debug.Log("3e malus: -15");
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            if (nbWrongAnswers == 4)
            {
                StartCoroutine(ShowMessage("Don't worry you will succeed the next one ", 4));
                score -= 20;
                //Debug.Log("4e malus: -20");
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            else if(nbWrongAnswers > 4)
            {
                //Debug.Log("Wrong answer given again.");
                StartCoroutine(ShowMessage("Wrong again! Stay focused and think harder.", 4));
                score -= 20;
                durationTimer = 0;
                overlay.color = new Color(overlay.color.r, overlay.color.g, overlay.color.b, 0.4f);
            }
            questionPopped = false;
            player.isAnswered = true;
            player.ableMoving = true;
            player.closedDoor = false;
        }



        CorrectChoice = false;
        //TransformScore();
        //Debug.Log(playerMark);
        tile.questionpoped = false;
        //Debug.Log("Check les réponses");



    } 

    // Close the panel question
    public void ClosePanel()
    {
        QuestionScreen.SetActive(false);
        questionPopped = false;


        //if (nbJokers != 0)
        //{
        //    nbJokers--;
        //    QuestionScreen.SetActive(false);
        //    questionPopped = false;

        //    grid._listTiles[tileX, tileY].tag = "Obstacle";

        //    StartCoroutine(ShowMessage("Careful, by skipping the question, you've used up 1 joker. You only have " + nbJokers.ToString() + " left now.", 8));
        //}

        //jokerText.text = "Joker : " + nbJokers.ToString() + "/3";

        //if (nbJokers == 0)
        //{
        //    CloseButton.SetActive(false);
        //}
        //tile.questionpoped = false;
        //Debug.Log("joker utilisé");

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


        //Debug.Log("Your mark is : " + playerMark + "/20");
    }
}
