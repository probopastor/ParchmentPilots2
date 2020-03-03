using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioClip gameMusic; 

    public GameObject pausePanel;
    public GameObject howToPlayPanel;

    public string mainMenuSceneName;

    public TestFlight thisFlight;

    private bool isPaused;
    
    // Start is called before the first frame update
    void Start()
    {
        thisFlight = FindObjectOfType<TestFlight>();

        MusicSource.clip = gameMusic;
        MusicSource.Play();

        isPaused = false;
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) && !thisFlight.throwing)
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        thisFlight.throwing = false;
        
        if (!isPaused)
        {
            isPaused = true;
            pausePanel.SetActive(true);
            howToPlayPanel.SetActive(false);
            Cursor.lockState = CursorLockMode.Confined;
            Time.timeScale = 0;
        }
        else if (isPaused)
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
