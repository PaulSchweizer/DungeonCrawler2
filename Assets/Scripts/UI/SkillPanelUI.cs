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
        if (Stats.SkillPoints >= SkillSlot.Skill.Cost(SkillSlot.Value))
        {
            RaiseButton.interactable = true;
        }
        else
        {
            RaiseButton.interactable = false;
        }
    }

    public void RaiseSkill()
    {
        bool success = Stats.RaiseSkill(SkillSlot.Skill);
        UpdateDisplay();
    }
}
