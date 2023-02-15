using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject titleScreen;
    public TextMeshProUGUI timeText;

    private float time = 0.0f; //timer
    // Start the game, remove title screen, reset timer, and adjust the questions based on difficulty button clicked
    public void StartGame(int difficulty)
    {
        // Voir comment ajuster la difficulté (question difficle, labyrinthe plus compliqué) 
        titleScreen.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateTime(); // faire en sorte que le timer commence après avoir cliqué sur le bouton
    }

    public void UpdateTime()
    {
        time += Time.deltaTime;
        timeText.text = "Timer: " + Mathf.Round(time); //refaire timer correctement, ainsi il y aurait bien heures, minutes et secondes
    }

    /* Restart game by reloading the scene
     * 
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }*/
}
