using ScriptableAttribute;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    public Text Name;
    public Slider HealthSlider;
    public Stats Stats;
    public FloatReference Health;

    private void Start()
    {
        UpdateHealth();
        Name.text = Stats.Name;
    }

    public void UpdateHealth()
    {
        HealthSlider.minValue = Health.MinValue;
        HealthSlider.maxValue = Health.MaxValue;
        HealthSlider.value = Health.Value;
    }
}
