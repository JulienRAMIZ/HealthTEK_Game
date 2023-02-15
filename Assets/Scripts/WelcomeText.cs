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
    public TextMeshProUGUI contextText;
    public float typingSpeed;
    //public AudioClip welcomeMusic;
    private int index;
    private float waitTime = 3.0f;

    // Start is called before the first frame update
    void Start()
    {
       // logoEstia.SetActive(true);
       // logoDonosti.SetActive(true);
        StartCoroutine(DisplayText());
        startButton.gameObject.SetActive(false);
        skipButton.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if ( index < contextSentences.Length) 
        {
            if (contextText.text == contextSentences[index])
            {
                NextSentence();
            }
            
        } 
        else 
        {
            StopAllCoroutines();
            startButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(false);
        } 

    }

    IEnumerator DisplayText()
    {
        foreach(char letter in contextSentences[index].ToCharArray())
        {
            contextText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator WaitThenDisplay()
    {
        yield return new WaitForSeconds(waitTime);
        contextText.text = "";
        StartCoroutine(DisplayText());
    }

    // Display the next sentence of a coroutine 

    public void NextSentence()
    {
        if (index < contextSentences.Length)
        {
            index++;
            StartCoroutine(WaitThenDisplay());   

        }
        /*else
        {
            Debug.Log("done");
            contextText.text = "";
            startButton.gameObject.SetActive(true);
            skipButton.gameObject.SetActive(false);
        }*/
    }

    public void ChangeScreen()
    {
        welcomeScreen.SetActive(false);
        difficultyPanel.SetActive(true);
    }
}
