using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class OptionsController : MonoBehaviour
{
    public static OptionsController instance;
    public AudioMixer audioMixer;

    public static bool invertedVerticalControls = false;
    public static bool invertedHorizontalControls = false;
    private float volume = 0;
    private FieldOfViewScaler fovScale;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        fovScale = FindObjectOfType<FieldOfViewScaler>();
    }

    public void SwitchInvertedControls(bool isInverted)
    {
        invertedVerticalControls = isInverted;
    }

    public void SwitchHorizontalInvertedControls(bool isInverted)
    {
        invertedHorizontalControls = isInverted;
    }

    public void ChangeVolumeLevel(float vol)
    {
        audioMixer.SetFloat("volume", vol);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (resolutionIndex == 0)
        {
            Screen.SetResolution(960, 720, true);
        }
        else if(resolutionIndex == 1)
        {
            Screen.SetResolution(1280, 1024, true);
        }
        else if(resolutionIndex == 2)
        {
            Screen.SetResolution(1920, 1080, true);
        }
        else if(resolutionIndex == 3)
        {
            Screen.SetResolution(1680, 1050, true);
        }

        if(fovScale != null)
        {
            fovScale.ChangeFieldOfView();
        }
    }

    public void GetAspectRatio()
    {
        //TODO
    }

    public float GetVolume()
    {
        return volume;
    }

    public bool GetInvertedControlls()
    {
        return invertedVerticalControls;
    }

    public bool GetInvertedControllsHorizontal()
    {
        return invertedHorizontalControls;
    }
}
