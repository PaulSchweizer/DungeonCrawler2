using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SkillPanelUI : MonoBehaviour
{
    public Text Name;
    public Text Value;
    public Button RaiseButton;

    [Header("References")]
    public Stats Stats;
    public SkillSlot SkillSlot;

    public void UpdateDisplay()
    {
        Name.text = SkillSlot.Skill.Name;
        Value.text = Convert.ToString(SkillSlot.Value);
        if (Stats.SkillPoints > 0)
        {
            RaiseButton.interactable = false;
        }
        else
        {
            RaiseButton.interactable = true;
        }
    }

    public void RaiseSkill()
    {
        SkillSlot.Value += 1;
        UpdateDisplay();
    }
}
