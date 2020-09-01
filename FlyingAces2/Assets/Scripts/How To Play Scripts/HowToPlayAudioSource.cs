/*
* Author: William Nomikos, Grant Frey
* HowToPlayAudioSources.cs
* Maintains a single audio source throughout the How To Play scenes.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HowToPlayAudioSource : MonoBehaviour
{
    public static HowToPlayAudioSource instance;

    public string[] scenesToExistIn;
    private bool keepAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        keepAudioSource = false;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        keepAudioSource = false;

        for (int i = 0; i < scenesToExistIn.Length; i++)
        {
            if (currentScene == scenesToExistIn[i])
            {
                keepAudioSource = true;
            }
        }

        if (!keepAudioSource)
        {
            instance = null;
            Destroy(gameObject);
        }
    }
}
