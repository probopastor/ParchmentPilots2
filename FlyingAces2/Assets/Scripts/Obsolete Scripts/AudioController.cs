﻿/*
* Author: Grant Frey
* AudioController.cs
* [Obsolete] Handles the volume for all audio sources.
*/

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
            audSou.volume = OptionsController.instance.GetVolume() / 100;
        }
    }
}
