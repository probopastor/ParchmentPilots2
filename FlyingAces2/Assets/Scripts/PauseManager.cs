using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioSource SinglePitchSoundEffectSource;

    public AudioClip gameMusic;
    public AudioClip openPauseMenuSound;
    public AudioClip buttonHoverSound;
    public AudioClip buttonPressSound;

    public GameObject pausePanel;
    public GameObject howToPlayPanel;

    public string mainMenuSceneName;

    public TestFlight thisFlight;

    public bool isPaused;

    public GameObject resumeButton;

    private EventSystem eventSystem;
    
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        thisFlight = FindObjectOfType<TestFlight>();
        eventSystem = EventSystem.current;

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
        OpenCloseMenuSound();

        if (!isPaused)
        {
            isPaused = true;
            pausePanel.SetActive(true);
            howToPlayPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(resumeButton);
            //Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 0;
        }
        else if (isPaused)
        {
            isPaused = false;
            pausePanel.SetActive(false);
            howToPlayPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
            //Cursor.lockState = CursorLockMode.Confined;
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

    public void OpenCloseMenuSound()
    {
        SinglePitchSoundEffectSource.clip = openPauseMenuSound;
        SinglePitchSoundEffectSource.Play();
    }

    public void ButtonHoverSound()
    {
        SinglePitchSoundEffectSource.clip = buttonHoverSound;
        SinglePitchSoundEffectSource.Play();
    }

    public void ButtonClick()
    {
        SinglePitchSoundEffectSource.clip = buttonPressSound;
        SinglePitchSoundEffectSource.Play();
    }
}
