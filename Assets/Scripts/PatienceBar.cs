using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PatienceBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;
    public int defaultMaxHealth = 100;
    void Awake()
    {
        // Initialize with default max health
        slider.maxValue = defaultMaxHealth;
        slider.value = defaultMaxHealth;
        fill.color = gradient.Evaluate(1f);
    }

    [System.Obsolete("SetMaxHealth is deprecated. Default max health is now 100.")]
    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public int GetHealth()
    {
        return (int)slider.value;
    }
}
