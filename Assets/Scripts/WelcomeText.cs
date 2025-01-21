using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/* Script that manages the welcoming panel when the game starts */
public class WelcomeText : MonoBehaviour
{
    public GameObject welcomeScreen;
    public GameObject timer;
    public GameObject ScoreText;
    public GameObject RulesButton;
    public GameObject tryAgainButton;
    public string[] contextSentences;
    public Button startButton;
    public Button skipButton;
    public Button nextButton;
    public Button previousButton;
    public GameObject ruleText;
    public GameObject ruleGrid;
    public GameObject notificationText;

    private int indexNext = 0;

    // public GameObject logoEstia;
    // public GameObject logoDonosti;
    // public GameObject difficultyPanel; // for a possible evolution of the game, players can choose the difficulty

    // Start is called before the first frame update
    void Start()
    {
        // logoEstia.SetActive(true);
        // logoDonosti.SetActive(true);
        startButton.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(false);
        ruleGrid.SetActive(false);
        timer.SetActive(false); //timer.gameObject.SetActive(false)
        ScoreText.SetActive(false); // ScoreText.gameObject.SetActive(false)
        RulesButton.SetActive(false);
        tryAgainButton.SetActive(false);
        notificationText.SetActive(false);
        //contextText.text = contextSentences[indexNext];

    }

    // Update is called once per frame
    void Update()
    {
        if (indexNext <= 0)
        {
            previousButton.gameObject.SetActive(false);
        }
        else
        {
            previousButton.gameObject.SetActive(true);
        }

        if (welcomeScreen.activeSelf == true)
        {
            notificationText.SetActive(false);
        }
        else { notificationText.SetActive(true);
        }
    }

    /* Display the next explanation whenever the user clicks on the "Next" button
       If we reach the last explanation, the user can directly press "Start" */
    //public void NextSentence()
    //{
    //    if (indexNext < contextSentences.Length && indexNext >= 0)
    //    {
    //        indexNext++;
    //        contextText.text = contextSentences[indexNext];
    //    }
    //    if (indexNext == contextSentences.Length-1)
    //    {
    //        startButton.gameObject.SetActive(true);
    //        skipButton.gameObject.SetActive(false);
    //        nextButton.gameObject.SetActive(false);
    //    }
    //}

    // Display the previous explanation whenever the user clicks on the "Previous" button
    //public void PreviousSentence()
    //{
    //    indexNext--;
    //    contextText.text = contextSentences[indexNext];


    //    if (indexNext <= 0)
    //    {
    //        indexNext = 0;
    //    }
    //    if (nextButton.gameObject.activeSelf == false)
    //    {
    //        nextButton.gameObject.SetActive(true);
    //    }
    //}

    // Display the scene where the player starts the game
    public void ChangeScreen()
    {
        welcomeScreen.SetActive(false);
        timer.SetActive(true);
        ScoreText.SetActive(true);
        RulesButton.SetActive(true);
        tryAgainButton.SetActive(true);
        notificationText.SetActive(true);
    }

    public void GoToRuleGrid()
    {
        ruleText.SetActive(false);
        nextButton.gameObject.SetActive(false);

        ruleGrid.SetActive(true);
        previousButton.gameObject.SetActive(true);

    }

    public void GoToRuleText()
    {
        ruleGrid.SetActive(false);
        previousButton.gameObject.SetActive(false);

        ruleText.SetActive(true);
        nextButton.gameObject.SetActive(true);
    }

}
