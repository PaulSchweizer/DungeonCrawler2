using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    public Sprite Portrait;
    public string Type;
    public string[] EnemyTypes;

    [Header("Attributes")]
    public ScriptableAttribute.FloatReference Health;
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
    public AspectDatabase AspectDatabase;
    public CharacterSet CharacterSet;
    public CharacterSet EnemySet;

    [Header("Events")]
    public GameEvent ItemEquipped;
    public GameEvent ItemUnEquipped;

    // Internals
    private List<Armour> _equippedArmour = new List<Armour>();

    #region Aspects and Skills

    public int SkillValueModifiers(Skill skill, string[] tags)
    {
        int modifiers = 0;
        foreach (Aspect aspect in AspectsAffectingSkill(skill))
        {
            if (aspect.Matches(tags) > 0)
            {
                modifiers += aspect.Bonus;
            }
        }
        return modifiers;
    }

    public int SkillValue(Skill skill, string[] tags)
    {
        int value = 0;
        for (int i = 0; i < Skills.Length; i++)
        {
            if (Skills[i].Skill == skill)
            {
                value = Skills[i].Value;
                break;
            }
        }
        value += SkillValueModifiers(skill, tags);
        return value;
    }

    public List<Aspect> AspectsAffectingSkill(Skill skill)
    {
        List<Aspect> aspects = new List<Aspect>();
        foreach (Aspect aspect in AllAspects)
        {
            if (Array.Exists(aspect.Skills, element => element == skill))
            {
                aspects.Add(aspect);
            }
        }
        return aspects;
    }

    public List<Aspect> AllAspects
    {
        get
        {
            List<Aspect> aspects = new List<Aspect>();

            // Basic Aspects
            if (Aspects != null)
            {
                foreach (Aspect aspect in Aspects)
                {
                    aspects.Add(aspect);
                }
            }

            // Aspects of all taken Consequences
            //foreach (Consequence consequence in AllConsequences)
            //{
            //    if (consequence.IsTaken)
            //    {
            //        aspects.Add(consequence.Effect);
            //    }
            //}

            // Aspects from the equipped Items
            foreach(EquipmentSlot slot in Equipment)
            {
                if (slot.Item != null)
                {
                    foreach (Aspect aspect in slot.Item.Aspects)
                    {
                        aspects.Add(aspect);
                    }
                }
            }

            return aspects;
        }
    }

    #endregion

    #region XP & Level

    public int Cost
    {
        get
        {
            int cost = 0;
            foreach (SkillSlot slot in Skills)
            {
                cost += slot.Value;
            }
            //foreach (Consequence consequence in AllConsequences)
            //{
            //    cost += consequence.Capacity;
            //}
            cost += Protection;
            cost += Damage;
            cost += (int)Health.MaxValue;
            return cost;
        }
    }

    public int Level
    {
        get
        {
            return (int)Math.Sqrt(XP / 100);
        }
    }

    public int XPForLevel(int level)
    {
        return level * level * 100;
    }

    public void ReceiveXP(int xp)
    {
        //GameEventsLogger.LogReceivesXP(this, xp);
        int previousLevel = Level;
        XP += xp;
        if (previousLevel < Level)
        {
            NextLevelReached();
        }
    }

    public void NextLevelReached()
    {
        //GameEventsLogger.LogReachesNextLevel(this);
        SkillPoints += 1;
    }

    #endregion

    #region Equipment

    public void Equip(Item item, EquipmentSlot slot)
    {
        slot.Item = item;
        ItemEquipped.Raise();
    }

    public void UnEquip(Item item, EquipmentSlot slot)
    {
        slot.Item = null;
        ItemUnEquipped.Raise();
    }
    
    public EquipmentSlot EquipmentSlotByName(string name)
    {
        foreach (EquipmentSlot slot in Equipment)
        {
            if (slot.Name == name) return slot;
        }
        return null;
    }

    public List<Item> EquippedItems
    {
        get
        {
            List<Item> items = new List<Item>();
            foreach (EquipmentSlot slot in Equipment)
            {
                items.Add(slot.Item);
            }
            return items;
        }
    }

    public Weapon EquipppedWeapon
    {
        get
        {
            foreach(EquipmentSlot slot in Equipment)
            {
                if(slot.Item is Weapon)
                {
                    return slot.Item as Weapon;
                }
            }
            return null;
        }
    }

    public List<Armour> EquipppedArmour
    {
        get
        {
            _equippedArmour.Clear();
            foreach (EquipmentSlot slot in Equipment)
            {
                if (slot.Item is Armour)
                {
                    _equippedArmour.Add(slot.Item as Armour);
                }
            }
            return _equippedArmour;
        }
    }

    #endregion

    #region Attributes

    public int Damage
    {
        get
        {
            int damage = 0;
            if (EquipppedWeapon != null)
            {
                damage += EquipppedWeapon.Damage;
            }
            return damage;
        }
    }

    public int Protection
    {
        get
        {
            int protection = 0;
            foreach (Armour armour in EquipppedArmour)
            {
                protection += armour.Protection;
            }
            return protection;
        }
    }

    #endregion

    #region Serialization

    public Dictionary<string, object> SerializeToData()
    {
        Dictionary<string, object> data = new Dictionary<string, object>();

        // Simple Attributes
        data["Name"] = Name;
        data["Health"] = Health;
        data["Type"] = Type;
        data["EnemyTypes"] = EnemyTypes;

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
            if (slot.Item != null) equipment[slot.Name] = slot.Item.Identifier;
            else equipment[slot.Name] = null;
        }
        data["Equipment"] = equipment;

        // Aspects and Tags
        data["Tags"] = Tags;
        List<string> aspects = new List<string>();
        foreach (Aspect aspect in Aspects)
        {
            aspects.Add(aspect.Name);
        }
        data["Aspects"] = aspects;

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
        Health.DeserializeFromData(SerializationUtilitites.DeserializeFromObject<Dictionary<string, object>>(data["Health"]));
        EnemyTypes = SerializationUtilitites.DeserializeFromObject<string[]>(data["EnemyTypes"]);

        List<SkillSlot> skills = new List<SkillSlot>();
        foreach (KeyValuePair<string, int> entry in SerializationUtilitites.DeserializeFromObject<Dictionary<string, int>>(data["Skills"]))
        {
            skills.Add(new SkillSlot(SkillDatabase.SkillByName(entry.Key), entry.Value));
        }
        Skills = skills.ToArray();

        // Equipment
        List<EquipmentSlot> equipment = new List<EquipmentSlot>();
        foreach (KeyValuePair<string, object> entry in SerializationUtilitites.DeserializeFromObject<Dictionary<string, object>>(data["Equipment"]))
        {
            equipment.Add(new EquipmentSlot(entry.Key, ItemDatabase.ItemByIdentifier(Convert.ToString(entry.Value))));
        }
        Equipment = equipment.ToArray();

        // Aspects and Tags
        Tags = SerializationUtilitites.DeserializeFromObject<List<string>>(data["Tags"]);
        Aspects = new List<Aspect>();
        foreach (string name in SerializationUtilitites.DeserializeFromObject<string[]>(data["Aspects"]))
        {
            Aspects.Add(AspectDatabase.AspectByName(name));
        }

        // Other data
        XP = Convert.ToInt32(data["XP"]);
        SkillPoints = Convert.ToInt32(data["SkillPoints"]);
        Radius = (float)Convert.ToDouble(data["Radius"]);
        AlertnessRadius = (float)Convert.ToDouble(data["AlertnessRadius"]);
    }

    #endregion
}