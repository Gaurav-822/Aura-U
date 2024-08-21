using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ZoomSlider : MonoBehaviour
{
    public Slider mySlider;
    void Start()
    {
        if (mySlider != null)
        {
            mySlider.value = 0.5f;
            mySlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
    }

    void OnSliderValueChanged(float value)
    {
        // when Slider value changes
    }

}
