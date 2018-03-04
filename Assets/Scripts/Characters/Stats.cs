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

[CreateAssetMenu(fileName = "Stats", menuName = "DungeonCrawler/Stats")]
public class Stats : ScriptableObject
{
    public string Name;

    [Header("Attributes")]
    public Attribute Health;
    public Skill[] Skills;
    public EquipmentSlot[] Equipment;

    [Header("Level related")]
    public int XP;
    public int SkillPoints;

    [Header("Other Attributes")]
    public float Radius;
    public float AlertnessRadius;

    [Header("References")]
    public ItemDatabase ItemDatabase;
    public SkillDatabase SkillDatabase;

    public Dictionary<string, object> SerializeToData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        // Attributes
        data["Name"] = Name;
        data["Health"] = Health;

        // Skills
        List<string> skills = new List<string>();
        foreach (Skill skill in Skills)
        {
            skills.Add(skill.Name);
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
        Health.DeserializeFromData(SerializationUtilitites.DeserializeFromObject(data["Health"]));

        // Skills
        List<Skill> skills = new List<Skill>();
        string json = JsonConvert.SerializeObject(data["Skills"]);
        foreach(string skill in JsonConvert.DeserializeObject<string[]>(json))
        {
            skills.Add(SkillDatabase.SkillByName(skill));
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
        Radius = Convert.ToInt32(data["Radius"]);
        AlertnessRadius = Convert.ToInt32(data["AlertnessRadius"]);
    }
}