using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject howToPlayPanel;

    public string mainMenuSceneName;

    private bool isPaused;
    

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if(!isPaused)
        {
            isPaused = true;
            pausePanel.SetActive(true);
            howToPlayPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }
        else if(isPaused)
        {
            isPaused = false;
            pausePanel.SetActive(false);
            howToPlayPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void RestartGame()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        Scene thisScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(thisScene.name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
