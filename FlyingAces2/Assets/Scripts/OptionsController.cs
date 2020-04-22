using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsController : MonoBehaviour
{
    public static OptionsController instance;

    public TMP_Dropdown dropdown;

    private bool invertedControls = false;
    private float volume = 100;

    public Slider volumeSlider;

    private AspectRatio aspectRatio;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchInvertedControls()
    {
        invertedControls = !invertedControls;
    }

    public void ChangeVolumeLevel()
    {
        volume = volumeSlider.value;
    }

    public void EditAspectRatio()
    {
        if(dropdown.value == 0)
        {
            aspectRatio = AspectRatio.Aspect4by3;
            SetAspectRatio(aspectRatio);
        }
        else if(dropdown.value == 1)
        {
            aspectRatio = AspectRatio.Aspect5by4;
            SetAspectRatio(aspectRatio);
        }
        else if(dropdown.value == 2)
        {
            aspectRatio = AspectRatio.Aspect16by9;
            SetAspectRatio(aspectRatio);
        }
        else if(dropdown.value == 3)
        {
            aspectRatio = AspectRatio.Aspect16by10;
            SetAspectRatio(aspectRatio);
        }
    }

    private void SetAspectRatio(AspectRatio newAspectRatio)
    {
        SetAspectRatio(newAspectRatio);
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
        return invertedControls;
    }
}
