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
    public string Description;
    public Sprite Sprite;
    public Aspect[] Aspects;
    public string[] Tags;
    public GameObject Prefab;
    public AspectDatabase AspectDatabase;

    [JsonIgnore]
    public string Identifier
    {
        get
        {
            return string.Format("{0}-{1}", Name, Id);
        }
    }

    public virtual void DeserializeFromJson(string json)
    {
        Dictionary<string, object> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json);
        Id = Convert.ToString(data["Id"]);
        Name = Convert.ToString(data["Name"]);
        Description = Convert.ToString(data["Description"]);
        List<Aspect> aspects = new List<Aspect>();
        foreach (string aspectName in SerializationUtilitites.DeserializeFromObject<string[]>(data["Aspects"]))
        {
            Aspect aspect = AspectDatabase.AspectByName(aspectName);
            if (aspect != null)
            {
                aspects.Add(aspect);
            }
        }
        Aspects = aspects.ToArray();
        Tags = SerializationUtilitites.DeserializeFromObject<string[]>(data["Tags"]);
    }
}
