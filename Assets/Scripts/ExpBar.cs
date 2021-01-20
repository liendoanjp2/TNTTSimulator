using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpBar : MonoBehaviour
{
    public Slider slider;
    private float maxExp = 1f;

    public void addExp(float exp)
    {
        if(slider.value < maxExp)
        {
            slider.value += exp;
        }

        //Check if we are over max, cap it
        if (slider.value > maxExp)
        {
            slider.value = maxExp;
        }

    }

    public float getMaxExp()
    {
        return maxExp - .26f;
    }

}
