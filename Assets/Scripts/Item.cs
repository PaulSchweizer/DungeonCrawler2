using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "Item", menuName = "DungeonCrawler/Item")]
public class Item : ScriptableObject
{
    public string Id;
    public string Name;

    [JsonIgnore]
    public string Identifier
    {
        get
        {
            return string.Format("{0}-{1}", Name, Id);
        }
    }

    public static Item DeserializeFromJson(string json)
    {
        return JsonConvert.DeserializeObject<Item>(json);
    }
}
