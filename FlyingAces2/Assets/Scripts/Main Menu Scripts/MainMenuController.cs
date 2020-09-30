/*
* Author: William Nomikos, Grant Frey
* MainMenuController.cs
* Handles main menu UI functionality, including keeping track of the 
* selected buttons, button sound effects, and main menu scene transitions. 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Animator transitionAnimator;

    public AudioSource MusicSource;
    public AudioSource UISoundEffectSource;
    public AudioClip mainMenuMusic;
    public AudioClip buttonPressSound;
    public AudioClip buttonHoverSound;

    private GameObject selectedButton;

    EventSystem eventSystem;
    private bool keepButtonSelected;

    public Color selectedButtonColor;
    public Color unSelectedButtonColor;

    public GameObject[] buttons;
    public Image[] buttonImages;
    public TextMeshProUGUI backButtonTmp;

    // Start is called before the first frame update
    void Start()
    {
        keepButtonSelected = false;
        eventSystem = EventSystem.current;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (MusicSource != null)
        {
            MusicSource.clip = mainMenuMusic;
            MusicSource.Play();
        }

        if(MusicSource == null)
        {
            MusicSource = FindObjectOfType<HowToPlayAudioSource>().GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        if(keepButtonSelected)
        {
            SetButton();
        }

        if (Input.GetKeyDown(KeyCode.Equals))
        {
            SaveData.UnlockAllLevels();
        }
    }

    public void StartLevel(string level)
    {
        StartCoroutine(LevelLoad(level));
        //SceneManager.LoadScene(level);
    }

    #region Open Functions (LeveSelect/Credits/Options/HowToPlay)
    public void OpenLevelSelect()
    {

    }

    public void OpenCredits(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void OpenOptions(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void OpenHowToPlay(string level)
    {
        SceneManager.LoadScene(level);
    }

    #endregion

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
   
    public void UderlineText(TextMeshProUGUI tmp)
    {
        tmp.fontStyle = FontStyles.Underline;
    }

    public void UnunderlineText(TextMeshProUGUI tmp)
    {
        tmp.fontStyle = FontStyles.Normal;
    }

    public void ColorBackground(Image thisObject)
    {
        thisObject.color = selectedButtonColor;
    }

    public void UncolorBackground(Image thisObject)
    {
        thisObject.color = unSelectedButtonColor;
    }

    public void ButtonHoverSound()
    {
        UISoundEffectSource.clip = buttonHoverSound;
        UISoundEffectSource.Play();
    }

    public void ButtonClick()
    {
        UISoundEffectSource.clip = buttonPressSound;
        UISoundEffectSource.Play();
    }

    public void SelectBackButton(GameObject button)
    {
        selectedButton = button;
        if(buttons != null)
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] == button)
                {
                    if (buttonImages[i] != null)
                    {
                        if (i == 3)
                        {
                            UderlineText(backButtonTmp);
                        }
                        else
                        {
                            buttonImages[i].color = selectedButtonColor;
                        }
                    }
                }
                else if (buttons[i] != button)
                {
                    if (buttonImages[i] != null)
                    {
                        if (i == 3)
                        {
                            UnunderlineText(backButtonTmp);
                        }
                        else
                        {
                            buttonImages[i].color = unSelectedButtonColor;
                        }
                    }
                }
            }
        }
        

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


    IEnumerator LevelLoad(string level)
    {
        transitionAnimator.SetTrigger("Start");

        yield return new WaitForSeconds(1);

        AsyncOperation operation = SceneManager.LoadSceneAsync(level);

        while(!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            yield return null;
        }

        //yield return new WaitForSeconds(1);
        //SceneManager.LoadScene(level);
    }

    public void ClearSaveData()
    {
        PlayerPrefs.SetInt("Level 1-1 high score", 0);
        PlayerPrefs.SetInt("level2Unlock", 0);
        PlayerPrefs.SetInt("level 1-2 high score", 0);
        PlayerPrefs.SetInt("level3Unlock", 0);
        PlayerPrefs.SetInt("level 1-3 high score", 0);
        PlayerPrefs.SetInt("level4Unlock", 0);
        PlayerPrefs.SetInt("level 1-4 high score", 0);

        PlayerPrefs.SetInt("Level 1-1 bobblehead 1", 0);
        PlayerPrefs.SetInt("Level 1-1 bobblehead 2", 0);
        PlayerPrefs.SetInt("Level 1-1 bobblehead 3", 0);

        PlayerPrefs.SetInt("level 1-2 bobblehead 1", 0);
        PlayerPrefs.SetInt("level 1-2 bobblehead 2", 0);
        PlayerPrefs.SetInt("level 1-2 bobblehead 3", 0);

        PlayerPrefs.SetInt("level 1-3 bobblehead 1", 0);
        PlayerPrefs.SetInt("level 1-3 bobblehead 2", 0);
        PlayerPrefs.SetInt("level 1-3 bobblehead 3", 0);

        PlayerPrefs.SetInt("level 1-4 bobblehead 1", 0);
        PlayerPrefs.SetInt("level 1-4 bobblehead 2", 0);
        PlayerPrefs.SetInt("level 1-4 bobblehead 3", 0);
    }
}
