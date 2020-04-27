using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseManager : MonoBehaviour
{
    public Animator transitionAnimator;

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

    private GameObject selectedButton;
    private bool keepButtonSelected;

    //tutorial things
    public GameObject tutorialAimObject;
    public GameObject tutorialThrowingObject;
    public GameObject tutorialChargingObject;
    public GameObject tutorialFlyingObject;

    
    // Start is called before the first frame update
    void Start()
    {
        keepButtonSelected = false;
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

        if(keepButtonSelected && isPaused)
        {
            SetButton();
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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
        else if (isPaused)
        {
            keepButtonSelected = false;
            isPaused = false;
            pausePanel.SetActive(false);
            howToPlayPanel.SetActive(false);
            eventSystem.SetSelectedGameObject(null);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1;
        }
    }

    public void SelectBackButton(GameObject button)
    {
        selectedButton = button;
        //eventSystem.SetSelectedGameObject(button);
        SetButton();
    }

    private void SetButton()
    {
        if (!EventSystem.current.alreadySelecting)
        {
            EventSystem.current.SetSelectedGameObject(selectedButton);
        }
    }

    public void OnMouseEnterUI()
    {
        keepButtonSelected = true;
    }

    public void OnMouseExitUI()
    {
        keepButtonSelected = false;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Scene thisScene = SceneManager.GetActiveScene();
        StartCoroutine(LevelLoad(thisScene.name));
        //SceneManager.LoadScene(thisScene.name);
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(LevelLoad(mainMenuSceneName));
        //SceneManager.LoadScene(mainMenuSceneName);
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


    IEnumerator LevelLoad(string level)
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(level);
    }
}
