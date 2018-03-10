using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "SkillDatabase", menuName = "DungeonCrawler/SkillDatabase")]
public class SkillDatabase : ScriptableObject
{
    public Skill[] Skills;

    public Skill SkillByName(string name)
    {
        for (int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i].Name== name)
            {
                return Skills[i];
            }
        }
        return null;
    }
}