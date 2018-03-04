using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "DungeonCrawler/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public Item[] Items;

    public T ItemByName<T>(string name)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Name == name)
            {
                return (T)Convert.ChangeType(Items[i], typeof(T));
            }
        }
        return default(T);
    }

    public T ItemByIdentifier<T>(string identifier)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Identifier == identifier)
            {
                return (T)Convert.ChangeType(Items[i], typeof(T));
            }
        }
        return default(T);
    }

    public Item ItemByIdentifier(string identifier)
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].Identifier == identifier)
            {
                return Items[i];
            }
        }
        return null;
    }
}

