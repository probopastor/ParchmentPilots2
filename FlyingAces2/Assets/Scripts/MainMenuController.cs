using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuController : MonoBehaviour
{
    GameObject[] mainMenu;
    EventSystem eventSystem;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GameObject.FindGameObjectsWithTag("MainMenu");
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void OpenMainMenu()
    {

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

    public void OpenHowToPlay()
    {

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
}
