using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//Script that manages the welcoming message at the beginning of the game
public class WelcomeText : MonoBehaviour
{

    // public GameObject logoEstia;
    // public GameObject logoDonosti;
    public GameObject welcomeScreen;
    public GameObject difficultyPanel;
    public string[] contextSentences;
    public Button startButton;
    public Button skipButton;
    public Button nextButton;
    public Button previousButton;
    public TextMeshProUGUI contextText;
    private int indexNext;

    // Start is called before the first frame update
    void Start()
    {
        // logoEstia.SetActive(true);
        // logoDonosti.SetActive(true);
        startButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true);
        nextButton.gameObject.SetActive(true);
        previousButton.gameObject.SetActive(false);
        NextSentence();
    }

    // Update is called once per frame
    void Update()
    {
        if (indexNext <= 1)
        {
            previousButton.gameObject.SetActive(false); 
        }

        else 
        {
            previousButton.gameObject.SetActive(true);    
        }
    }

    /* Display the next explanation whenever the user clicks on the "Next" button
       If we reach the last explanation, the user can directly presses "Start" */
    public void NextSentence()
    {
        if (indexNext < contextSentences.Length && indexNext >= 0)
        {
            contextText.text = contextSentences[indexNext];
            indexNext++;
        }
       
        if (indexNext == contextSentences.Length)
        {
            Debug.Log("done");
            startButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(false);
        }
        
        Debug.Log(indexNext);
    }

    // Display the previous explanation whenever the user clicks on the "Previous" button
    public void PreviousSentence()
    {
        contextText.text = contextSentences[indexNext-2];
        Debug.Log(contextSentences[indexNext-2]);
        indexNext --;
        if (indexNext <= 0)
        {
            indexNext = 0;
        }
        if (nextButton.gameObject.activeSelf == false)
        {
            nextButton.gameObject.SetActive(true);
        }
    }

    // Display the screen where the user can set the game difficulty
    public void ChangeScreen()
    {
        welcomeScreen.SetActive(false);
        difficultyPanel.SetActive(true);
    }
}
