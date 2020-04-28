using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

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

    // Start is called before the first frame update
    void Start()
    {
        keepButtonSelected = false;
        eventSystem = EventSystem.current;

        if(MusicSource != null)
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

    public void OpenCredits()
    {

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
}
