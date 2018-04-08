using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPortrait : MonoBehaviour
{
    public Text Name;
    public Slider HealthSlider;
    public Stats Stats;

    private void Start()
    {
        UpdateView();
    }

    public void UpdateView()
    {
        Name.text = Stats.Name;
        HealthSlider.minValue = Stats.Health.MinValue;
        HealthSlider.maxValue = Stats.Health.MaxValue;
        HealthSlider.value = Stats.Health.Value;
    }
}
