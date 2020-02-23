using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowingChargeBarController : MonoBehaviour
{
    public Slider chargeBar;

    private bool isEnabled;

    public float sliderIncrementValue = 0.1f;
    private float sliderValue = 0;

    private void OnEnable()
    {
        sliderValue = 0;
        StartCoroutine("UpdateSlider");
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateSlider()
    {
        sliderValue += sliderIncrementValue;
        chargeBar.value = Mathf.PingPong(sliderValue * 3, 100);
        yield return new WaitForSecondsRealtime(0.001f);
        StartCoroutine("UpdateSlider");
    }

}

