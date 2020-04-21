using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    AudioSource[] audioSources;
    // Start is called before the first frame update
    void Start()
    {
        audioSources = FindObjectsOfType<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (AudioSource audSou in audioSources)
        {
            audSou.volume = OptionsController.volume / 100;
        }
    }
}
