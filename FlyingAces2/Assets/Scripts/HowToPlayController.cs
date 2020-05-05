/*
* Author: Grant Frey
* HowToPlayController.cs
* Handles switching between How To Play scenes and the button underlining 
* in those scenes upon buttons being selected.
*/

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

    public Animator transitionAnimator;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }

    public void GoBackToMainMenu(string scene)
    {
        StartCoroutine(LevelLoad(scene));
        //SceneManager.LoadScene(scene);
    }

    public void OpenNextPage(string scene)
    {
        StartCoroutine(LevelLoad(scene));
        //SceneManager.LoadScene(scene);
    }

    public void OpenPreviousPage(string scene)
    {
        StartCoroutine(LevelLoad(scene));
        //SceneManager.LoadScene(scene);
    }

    public void UderlineText(TextMeshProUGUI tmp)
    {
        tmp.fontStyle = FontStyles.Underline;
    }

    public void UnunderlineText(TextMeshProUGUI tmp)
    {
        tmp.fontStyle = FontStyles.Normal;
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
