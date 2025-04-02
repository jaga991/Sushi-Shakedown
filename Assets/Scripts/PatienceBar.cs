using UnityEngine;
using System.Collections;

using UnityEngine.UI;
public class PatienceBar : MonoBehaviour
{
    public Slider slider;

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHeath(int health)
    {
        slider.value = health;
    }

}
