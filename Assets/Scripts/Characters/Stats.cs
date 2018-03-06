using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EquipmentSlot
{
    public string Name;
    public Item Item;

    public EquipmentSlot() { }

    public EquipmentSlot(string name, Item item)
    {
        Name = name;
        Item = item;
    }
}

[Serializable]
public class SkillSlot
{
    public Skill Skill;
    public int Value;

    public SkillSlot() { }

    public SkillSlot(Skill skill, int value)
    {
        Skill = skill;
        Value = value;
    }

    [SerializeField]
    public string Name
    {
        get { return Skill.Name; }
    }
}

[CreateAssetMenu(fileName = "Stats", menuName = "DungeonCrawler/Stats")]
public class Stats : ScriptableObject
{
    public string Name;
    public string Type;
    public string[] EnemyTypes;

    [Header("Attributes")]
    public Attribute Health;
    public SkillSlot[] Skills;
    public EquipmentSlot[] Equipment;

    [Header("Level related")]
    public int XP;
    public int SkillPoints;

    [Header("Other Attributes")]
    public float Radius;
    public float AlertnessRadius;

    [Header("Aspects and Tags")]
    public List<string> Tags;
    public List<Aspect> Aspects;

    [Header("References")]
    public ItemDatabase ItemDatabase;
    public SkillDatabase SkillDatabase;

    public int SkillValue(Skill skill)
    {
        for(int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i].Skill == skill)
            {
                return Skills[i].Value;
            }
        }
        return 0;
    }

    #region Serialization

    public Dictionary<string, object> SerializeToData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        // Attributes
        data["Name"] = Name;
        data["Health"] = Health;
        data["Type"] = Type;

        // Skills
        Dictionary<string, int> skills = new Dictionary<string, int>();
        foreach (SkillSlot skill in Skills)
        {
            skills[skill.Skill.Name] = skill.Value;
        }
        data["Skills"] = skills;

        // Equipment
        Dictionary<string, string> equipment = new Dictionary<string, string>();
        foreach (EquipmentSlot slot in Equipment)
        {
            equipment[slot.Name] = slot.Item.Identifier;
        }
        data["Equipment"] = equipment;

        // Other data
        data["XP"] = XP;
        data["SkillPoints"] = SkillPoints;
        data["Radius"] = Radius;
        data["AlertnessRadius"] = AlertnessRadius;

        return data;
    }

    public void DeserializeFromData(Dictionary<string, object> data)
    {
        // Attributes
        Name = Convert.ToString(data["Name"]);
        Type = Convert.ToString(data["Type"]);
        Health.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Health"]));

        List<SkillSlot> skills = new List<SkillSlot>();
        foreach (KeyValuePair<string, object> entry in SerializationUtilitites.DeserializeFromObject(data["Skills"]))
        {
            skills.Add(new SkillSlot(SkillDatabase.SkillByName(entry.Key), Convert.ToInt32(entry.Value)));
        }
        Skills = skills.ToArray();

        // Equipment
        List<EquipmentSlot> equipment = new List<EquipmentSlot>();
        foreach (KeyValuePair<string, object> entry in SerializationUtilitites.DeserializeFromObject(data["Equipment"]))
        {
            equipment.Add(new EquipmentSlot(entry.Key, ItemDatabase.ItemByIdentifier(Convert.ToString(entry.Value))));
        }
        Equipment = equipment.ToArray();

        // Other data
        XP = Convert.ToInt32(data["XP"]);
        SkillPoints = Convert.ToInt32(data["SkillPoints"]);
        Radius = (float)Convert.ToDouble(data["Radius"]);
        AlertnessRadius = (float)Convert.ToDouble(data["AlertnessRadius"]);
    }

    #endregion
}