using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    public AudioSource MusicSource;
    public AudioSource UISoundEffectSource;
    public AudioClip mainMenuMusic;
    public AudioClip buttonPressSound;
    public AudioClip buttonHoverSound;

    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
        MusicSource.clip = mainMenuMusic;
        MusicSource.Play();
    }

    public void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void OpenLevelSelect()
    {

    }

    public void OpenCredits()
    {

    }

    public void OpenOptions()
    {

    }

    public void OpenHowToPlay(string level)
    {
        SceneManager.LoadScene(level);
    }

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
}
