using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class OptionsUIStuff : MonoBehaviour
{
    public Toggle verticleToggle;
    public Toggle horizontalToggle;
    public Slider volumeSlider;
    public TMP_Dropdown aspectRatioDropdown;
    public AudioMixer audioMixer;
    private float volume;

    // Start is called before the first frame update
    void Start()
    {
        verticleToggle.isOn = OptionsController.invertedVerticalControls;
        horizontalToggle.isOn = OptionsController.invertedHorizontalControls;
        audioMixer.GetFloat("volume", out volume);
        volumeSlider.value = volume;
        Resolution thisRes = Screen.currentResolution;
        if (thisRes.width == 960 && thisRes.height == 720)
        {
            aspectRatioDropdown.value = 0;
        }
        else if (thisRes.width == 1280 && thisRes.height == 1024)
        {
            aspectRatioDropdown.value = 1;
        }
        else if (thisRes.width == 1920 && thisRes.height == 1080)
        {
            aspectRatioDropdown.value = 2;
        }
        else if (thisRes.width == 1680 && thisRes.height == 1050)
        {
            aspectRatioDropdown.value = 3;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
