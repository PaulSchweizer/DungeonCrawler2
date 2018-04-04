using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "DungeonCrawler/Item")]
public class Item : ScriptableObject
{
    public string Id;
    public string Name;
    public Sprite Sprite;
    public Aspect[] Aspects;
    public GameObject Prefab;

    [JsonIgnore]
    public string Identifier
    {
        get
        {
            return string.Format("{0}-{1}", Name, Id);
        }
    }

    public void DeserializeFromJson(string json)
    {
        Dictionary<string, object> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json);
        Id = Convert.ToString(data["Id"]);
        Name = Convert.ToString(data["Name"]);
        // Find Sprite by name in the Folder Structure
        // Find Aspects in the AspectsDatabase
        // Find Prefab in the Folder Structure
    }
}
