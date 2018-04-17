using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SkillsUI : MonoBehaviour
{
    [Header("References")]
    public Stats Stats;

    [Header("Prefabs")]
    public SkillPanelUI SkillPanelPrefab;

    public RectTransform SkillPanelParent;

    private Dictionary<string, SkillPanelUI> SkillPanels = new Dictionary<string, SkillPanelUI>();

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        foreach(SkillSlot skillSlot in Stats.Skills)
        {
            SkillPanelUI panel;
            SkillPanels.TryGetValue(skillSlot.Skill.Name, out panel);
            if (panel == null)
            {
                panel = Instantiate(SkillPanelPrefab, SkillPanelParent);
                panel.Stats = Stats;
                panel.SkillSlot = skillSlot;
                SkillPanels[skillSlot.Skill.Name] = panel;
            }
            panel.UpdateDisplay();
        }
    }
}
