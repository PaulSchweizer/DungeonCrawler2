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
    public Aspect[] Aspects;

    [JsonIgnore]
    public string Identifier
    {
        get
        {
            return string.Format("{0}-{1}", Name, Id);
        }
    }
}
