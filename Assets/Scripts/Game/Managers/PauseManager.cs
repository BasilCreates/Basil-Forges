using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Settings Parameters")]
    public GameObject pausePanel;
    public GameObject settingsPanel;
    public GameObject gameUI;
    private bool isPaused = false;


    //Make sure the cursor is locked upon start
    public void Start()
    {
       Cursor.lockState = CursorLockMode.Locked;
    }

    //When you press p, the settings should open
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    //When you pause the game
    void PauseGame()
    {
        pausePanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;  
        Cursor.visible = true;                    
    }

    //When you Resume the Game
    public void ResumeGame()
    {
        pausePanel.SetActive(false);
        gameUI.SetActive(true);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false;
    }

    public void OnMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game Menu");
    }

    //When you open up the settings
    public void OnSettingsClicked()
    {
        pausePanel.SetActive(false );
        gameUI.SetActive( false );
        settingsPanel.SetActive(true);
        Time.timeScale = 0f;
        isPaused=true;

    }
    
    //When you click the return button in settings
    public void OnReturnClicked()
    {
        settingsPanel.SetActive(false);
        gameUI.SetActive(true);
        pausePanel.SetActive(true);
        Time.timeScale=0f;
        isPaused=true;
    }

}
