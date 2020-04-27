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
    void Awake()
    {
        keepAudioSource = false;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        string currentScene = SceneManager.GetActiveScene().name;

        for (int i = 0; i < scenesToExistIn.Length; i++)
        {
            if(currentScene == scenesToExistIn[i])
            {
                keepAudioSource = true;
            }
        }

        if(!keepAudioSource)
        {
            Destroy(this);
        }
    }
}
