/*
* Author: Bobby Kohls
* MainMenuAnimationSound.cs
* Plays animation sound effects when main menu animation occurs.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuAnimationSound : MonoBehaviour
{
    public AudioSource UIAudioSource;
    public AudioClip UISoundEffect;

    public void PlaySound()
    {
        UIAudioSource.clip = UISoundEffect;
        UIAudioSource.Play();
    }
}
