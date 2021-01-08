using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Text;
using UnityEngine.UI;



public class ScreenInfo : MonoBehaviour
{
    private Slider timeSlider;

    void Start()
    {
        timeSlider = GetComponent<Slider>();

    }

    void Update()
    {
        Time.timeScale = timeSlider.value;
    }
}
