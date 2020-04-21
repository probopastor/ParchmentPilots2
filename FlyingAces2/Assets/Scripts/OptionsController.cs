using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsController : MonoBehaviour
{
    public static bool invertedControls = false;
    public static float volume = 100;

    public Slider volumeSlider;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void switchInvertedControls()
    {
        invertedControls = !invertedControls;
        Debug.Log(invertedControls);
    }

    public void changeVolumeLevel()
    {
        volume = volumeSlider.value;
        Debug.Log(volume);
    }
}
