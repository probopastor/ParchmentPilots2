using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class HowToPlayController : MonoBehaviour
{
    EventSystem eventSystem;
    public GameObject previousButton;
    public GameObject nextButton;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    public void GoBackToMainMenu(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OpenNextPage(string scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void OpenPreviousPage(string scene)
    {
        SceneManager.LoadScene(scene);
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
