using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * @Special thanks to Brackeys (https://youtu.be/BLfNP4Sc_iA)
 */
public class HealthUIManager : MonoBehaviour
{
    public Slider healthSlider;
    public FloatVariable playerCurrentHealth;

    private void Start()
    {
        InitHealth();
    }

    public void InitHealth()
    {
        healthSlider.maxValue = playerCurrentHealth.InitialValue;
        healthSlider.value = playerCurrentHealth.InitialValue;
    }

    public void UpdateHealth()
    {
        healthSlider.value = playerCurrentHealth.RuntimeValue;
    }
}
