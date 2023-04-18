using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    // public TextMeshProUGUI TimeText;
    // public TextMeshProUGUI QuestionTitle;
    // private float time = 0.0f; //timer

    public GameObject TitleScreen;
    public TextMeshProUGUI QuestionText;
    public Button QuestionButton;
    public GameObject QuestionScreen;
    public GameObject[] Choices;
    public List<string> QnA;
    public Tile tile;
    private string CorrectAnswer;
    private GameObject SelectedButton;
    public bool CorrectChoice = false;
    //private readonly string FilePath = "C:/DEV/QnA.csv";
    private int RandomIndex;

    void Start()
    {
        string FilePath = Application.dataPath + "/QnA_Files/QnA.csv";
        QuestionScreen.SetActive(false);

        //Open the questions and answers file and retrieve the values
        var reader = new StreamReader(File.OpenRead(FilePath));
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(';');
            QnA.Add(values[0]);
            QnA.Add(values[1]);
            QnA.Add(values[2]);
        }
        QnA.RemoveRange(0, 3); //remove first values since they correspond to the first row of the CSV file (Question, Possible_Answer, Correct_Answer)
    }

    // Start the game, remove title screen, reset timer, and adjust the questions based on difficulty button clicked
    public void StartGame(int difficulty)
    {
        //Voir comment ajuster la difficulté (question difficle, labyrinthe plus compliqué) 
        TitleScreen.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
       // UpdateTime(); // faire en sorte que le timer commence après avoir cliqué sur le bouton
    }

    /*public void UpdateTime()
    {
        time += Time.deltaTime;
        TimeText.text = "Timer: " + Mathf.Round(time); //refaire timer correctement, ainsi il y aurait bien heures, minutes et secondes
    }*/

    public void PopUpQuestion() 
    {
        QuestionScreen.SetActive(true);
        QuestionButton.gameObject.SetActive(false);
        RandomIndex = UnityEngine.Random.Range(0, QnA.Count);
        QuestionText.text = QnA[RandomIndex];

        while (!QuestionText.text.EndsWith('?'))
        {
            RandomIndex = UnityEngine.Random.Range(0, QnA.Count);
            QuestionText.text = QnA[RandomIndex];
        }

        SetAnswers();
    }

    // Set the possible answers from the csv file to the choice buttons on Unity
    public void SetAnswers() 
    {
        for (int j = 0; j < 4; j++)
        {
            Choices[j].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = (QnA[RandomIndex + 1].Split(','))[j];
        }
        CorrectAnswer = QnA[RandomIndex + 2];
        Debug.Log(CorrectAnswer);
    }

    //Check if the player chose the correct answer
    public void IsCorrect()
    {
        CheckButton(Choices);
    }
    public void CheckButton(GameObject[] Choices)
    {
        SelectedButton = EventSystem.current.currentSelectedGameObject;
        Debug.Log(SelectedButton);
        for (int i = 0; i < Choices.Length; i++)
        {
            if (SelectedButton == Choices[i])
            {
                if (Choices[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == CorrectAnswer)
                {
                    Debug.Log("Correct Answer");
                    CorrectChoice = true;
                    tile.CorrectAnswer();
                    break;
                }
                else
                {
                    Debug.Log("Wrong Answer");
                    CorrectChoice = false;
                    //PUNITION POSSIBILITE DE RECOMMENCER DANS X MINUTES
                }
            }
        }
    }
    public void CheckCorrect()
    {
        if (CorrectChoice)
        {
            QuestionScreen.SetActive(false);
            QuestionButton.gameObject.SetActive(true);
        }
    }
    
   /* public void CheckCorrect(Button button)
    {
       if( button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text == CorrectAnswer)
       {
            Debug.Log("Correct Answer");
            QuestionScreen.SetActive(false);
            QuestionButton.gameObject.SetActive(true);
       }
       else 
       {
            Debug.Log("Wrong Answer");
       }
    }*/
    
    /* Restart game by reloading the scene
     * 
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }*/
}
