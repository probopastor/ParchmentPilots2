/*
* Author: William Nomikos
* PauseManager.cs
* Handles pause functionality, including maintaining which buttons 
* are selected, unpausing the game, restarting the game, and 
* the in-game how to play screen.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public GameObject endOfLevelPanel;

    public string mainMenuSceneName;

    public TestFlight thisFlight;
    public PlaneThrow_Handler planeThrow;

    public bool isPaused;

    public GameObject resumeButton;
    public GameObject nextLevelButton;

    private EventSystem eventSystem;

    private GameObject selectedButton;
    private bool keepButtonSelected;

    public Color selectedButtonColor;
    public Color unSelectedButtonColor;

    public Image[] allButtons;

    public InputMaster controls;

    private void Awake()
    {
        controls = new InputMaster();
        controls.Player.Pause.performed += context => PauseGame();
    }
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    // Start is called before the first frame update
    void Start()
    {
        keepButtonSelected = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        thisFlight = FindObjectOfType<TestFlight>();
        planeThrow = FindObjectOfType<PlaneThrow_Handler>();

        eventSystem = EventSystem.current;

        MusicSource.clip = gameMusic;
        MusicSource.Play();

        isPaused = false;
        pausePanel.SetActive(false);
        howToPlayPanel.SetActive(false);
        endOfLevelPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.Escape) && !planeThrow.GetThrowStatus())
        //{
        //    PauseGame();
        //}

        if(keepButtonSelected && isPaused)
        {
            SetButton();
        }
    }

    public void PauseGame()
    {
        if (!planeThrow.GetThrowStatus())
        {
            planeThrow.SetThrowStatus(false);

            OpenCloseMenuSound();

            if (!isPaused)
            {
                isPaused = true;

                for (int i = 0; i < allButtons.Length; i++)
                {
                    allButtons[i].color = unSelectedButtonColor;
                }
                allButtons[0].color = selectedButtonColor;

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
    }

    public void SelectBackButton(GameObject button)
    {
        selectedButton = button;
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

    public void ColorBackground(Image thisObject)
    {
        thisObject.color = selectedButtonColor;
    }

    public void UncolorBackground(Image thisObject)
    {
        thisObject.color = unSelectedButtonColor;
    }

    public void RestartGame()
    {
        Time.timeScale = 1;
        Scene thisScene = SceneManager.GetActiveScene();
        StartCoroutine(LevelLoad(thisScene.name));
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1;
        StartCoroutine(LevelLoad(mainMenuSceneName));
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

    public void LoadNextLevel(string level)
    {
        transitionAnimator.SetTrigger("Start");

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);
    }

    public void OpenEndOfLevelMenu()
    {
        endOfLevelPanel.SetActive(true);
        eventSystem.SetSelectedGameObject(nextLevelButton);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    IEnumerator LevelLoad(string level)
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }
    }
}
