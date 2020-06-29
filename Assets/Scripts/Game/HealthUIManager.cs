using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * 
 * @author ShifatKhan
 * @Special thanks to Brackeys (https://youtu.be/BLfNP4Sc_iA) for slider
 * @Special thanks to Code Monkey (https://youtu.be/cR8jP8OGbhM) for damaged bar
 */
public class HealthUIManager : MonoBehaviour
{
    // HP bar.
    public Slider healthSlider;
    public FloatVariable playerCurrentHealth;

    // Damage bar behind HP bar.
    public Slider damagedSlider;

    public float damagedShrinkTimer;
    private const float DAMAGED_SHRINK_TIMER_MAX = 1f;
    public float damagedShrinkAmount = 2f;

    // White fade on top of damage bar.
    public Slider damagedEffectSlider;
    public Image damagedEffectImage;
    private Color damagedEffectColor;

    public float damagedFadeTimer;
    private const float DAMAGED_FADE_TIMER_MAX = 0.1f;
    public float damagedFadeAmount = 5f;

    private void Awake()
    {
        damagedSlider = transform.parent.Find("Damaged Fill").GetComponent<Slider>();

        damagedEffectSlider = transform.parent.Find("Damaged Effect Fill").GetComponent<Slider>();
        damagedEffectImage = damagedEffectSlider.GetComponentInChildren<Image>();
        damagedEffectColor = damagedEffectImage.color;
        damagedEffectColor.a = 0;
        damagedEffectImage.color = damagedEffectColor;
    }

    private void Start()
    {
        InitHealth();
    }

    public void InitHealth()
    {
        healthSlider.maxValue = playerCurrentHealth.InitialValue;
        healthSlider.value = playerCurrentHealth.InitialValue;

        damagedSlider.maxValue = healthSlider.maxValue;
        damagedSlider.value = healthSlider.value;

        damagedEffectSlider.maxValue = healthSlider.maxValue;
        damagedEffectSlider.value = healthSlider.value;
    }

    private void Update()
    {
        // Damaged SHRINK effect.
        damagedShrinkTimer -= Time.deltaTime;
        if(damagedShrinkTimer < 0)
        {
            if(healthSlider.value < damagedSlider.value)
            {
                damagedSlider.value -= damagedShrinkAmount * Time.deltaTime;
            }
        }

        // Damaged FADE effect.
        if (damagedEffectColor.a > 0)
        {
            damagedFadeTimer -= Time.deltaTime;
            if(damagedFadeTimer < 0)
            {
                damagedEffectColor.a -= damagedFadeAmount * Time.deltaTime;
                damagedEffectImage.color = damagedEffectColor;
            }
        }
    }

    /** Called when player takes damage.
     */
    public void UpdateHealth()
    {
        if(damagedEffectColor.a <= 0)
        {
            // Bar is invisible.
            damagedEffectSlider.value = healthSlider.value;
        }

        // Make sure damaged color is visible.
        damagedEffectColor.a = 1;
        damagedEffectImage.color = damagedEffectColor;

        damagedFadeTimer = DAMAGED_FADE_TIMER_MAX;
        damagedShrinkTimer = DAMAGED_SHRINK_TIMER_MAX;

        healthSlider.value = playerCurrentHealth.RuntimeValue;
    }
}
