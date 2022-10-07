using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnrolledToggle : MonoBehaviour
{
    public Toggle enrolledbool;
    public Image toggleImage;

    public Slider bar;
    public TMP_Text barValue;

    void Update()
    {
        if (enrolledbool.isOn == true)
        {
            toggleImage.color = Color.green;
        }
        if (enrolledbool.isOn == false)
        {
            toggleImage.color = Color.red;
        }
        barValue.text = "id: " + bar.value;
    }
}
