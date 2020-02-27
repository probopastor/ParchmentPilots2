using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
        foreach(GameObject button in mainMenu)
        {
            button.SetActive(true);
        }
        //eventSystem.currentSelectedGameObject();
    }

    public void OpenLevelSelect()
    {
        foreach (GameObject button in mainMenu)
        {
            button.SetActive(false);
        }
    }

    public void OpenCredits()
    {
        foreach (GameObject button in mainMenu)
        {
            button.SetActive(false);
        }
    }

    public void OpenOptions()
    {
        foreach (GameObject button in mainMenu)
        {
            button.SetActive(false);
        }
    }

    public void OpenHowToPlay()
    {
        foreach (GameObject button in mainMenu)
        {
            button.SetActive(false);
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
