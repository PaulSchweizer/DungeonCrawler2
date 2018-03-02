using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "DungeonCrawler/ItemDatabase")]
class ItemDatabase : ScriptableObject
{
    public Item[] Items;
}

