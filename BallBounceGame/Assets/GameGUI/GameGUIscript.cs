﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class GameGUIscript : MonoBehaviour
{
    public GameObject MenuButton;
    public GameObject EndGameMenu;
    public GameObject PauseMenu;


    void RestartLevel()
    {
        Scene current_scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current_scene.name);
        Time.timeScale = 1f;
    }

    public void RestartButtonClicked()
    {
        RestartLevel();
    }

    public void FinishLevel()
    {
        MenuButton.SetActive(false);
        EndGameMenu.SetActive(true);
    }

    public void LoadNextLevel()
    {

        // Loads next available level (current max of 5)
        if( SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
       
    }
    public void ReturnToTitle()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void PauseGame()
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    public void UnpauseGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
        EndGameMenu.SetActive(false);
        PauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("r")) 
        {
            RestartLevel();
        }
    }
}
