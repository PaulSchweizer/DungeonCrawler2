using UnityEngine;
using System.Collections.Generic;
using SlotSystem;
using UnityEngine.UI;

public class StatusUI : MonoBehaviour
{
    public Stats Stats;

    public Text NameValue;
    public Text LevelValue;
    public Slider XPSlider;
    public Text XPValue;

    public void UpdateDisplay()
    {
        NameValue.text = Stats.Name;

        // Level
        LevelValue.text = Stats.Level.ToString();
        XPSlider.maxValue = Stats.XPForLevel(Stats.Level + 1) - Stats.XPForLevel(Stats.Level);
        XPSlider.minValue = 0;
        XPSlider.value = Stats.XP - Stats.XPForLevel(Stats.Level);
        XPValue.text = string.Format("{0}/{1}", XPSlider.value, XPSlider.maxValue); 
    }
}
