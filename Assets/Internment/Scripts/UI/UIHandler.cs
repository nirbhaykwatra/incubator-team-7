using System;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
    }

    public void SetupSlider(float max)
    {
        _slider.minValue = 0f;
        _slider.maxValue = max;
    }

    public void HandleSlider(float value)
    {
        _slider.value = value;
    }
}
