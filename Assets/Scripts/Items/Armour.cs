using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Armour", menuName = "DungeonCrawler/Armour")]
public class Armour : Item
{
    public int Protection;

    public override void DeserializeFromJson(string json)
    {
        base.DeserializeFromJson(json);
        Dictionary<string, object> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json);
        Protection = Convert.ToInt32(data["Protection"]);
    }
}
