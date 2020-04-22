using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public static OptionsController instance;

    private bool invertedControls = false;
    private float volume = 100;

    public Slider volumeSlider;
    private Vector2 thisResolution;

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

    public void ChangeResolution(Vector2 resolution)
    {
        thisResolution = resolution;
        Screen.SetResolution((int)resolution.x, (int)resolution.y, true);
    }

    public Vector2 GetResolution()
    {
        return thisResolution;
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
