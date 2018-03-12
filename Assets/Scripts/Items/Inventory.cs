using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class InventoryEntry
{
    public Item Item;
    public int Amount;

    public InventoryEntry() { }

    public InventoryEntry(Item item, int amount)
    {
        Item = item;
        Amount = amount;
    }
}

[CreateAssetMenu(fileName = "Inventory", menuName = "DungeonCrawler/Inventory")]
public class Inventory : ScriptableObject
{
    public ItemDatabase ItemDatabase;
    public List<InventoryEntry> Entries = new List<InventoryEntry>();

    public void AddItem(Item item, int amount = 1)
    {
        for(int i = 0; i < Entries.Count; i++)
        {
            if (Entries[i].Item.Id == item.Id)
            {
                Entries[i].Amount += amount;
                return;
            }
        }
        Entries.Add(new InventoryEntry(item, amount));
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        for (int i = Entries.Count - 1; i >= 0; i--)
        {
            if (Entries[i].Item.Id == item.Id)
            {
                Entries[i].Amount -= amount;
                if (Entries[i].Amount <= 0)
                {
                    Entries.Remove(Entries[i]);
                }
                return;
            }
        }
    }

    public int Amount(Item item)
    {
        for (int i = 0; i < Entries.Count; i++)
        {
            if (Entries[i].Item.Id == item.Id)
            {
                return Entries[i].Amount;
            }
        }
        return 0;
    }

    public void Clear()
    {
        Entries.Clear();
    }

    public static Inventory operator +(Inventory thisInventory, Inventory thatInventory)
    {
        foreach (InventoryEntry entry in thatInventory.Entries)
        {
            thisInventory.AddItem(entry.Item, thatInventory.Amount(entry.Item));
        }
        thatInventory.Clear();
        return thisInventory;
    }

    #region Serialization

    public Dictionary<string, int> SerializeToData()
    {
        Dictionary<string, int> data = new Dictionary<string, int>();
        for (int i = 0; i < Entries.Count; i++)
        {
            data[Entries[i].Item.Identifier] = Entries[i].Amount;
        }
        return data;
    }

    public void DeserializeFromData(Dictionary<string, object> data)
    {
        Clear();
        foreach(KeyValuePair<string, object> entry in data)
        {
            Item item = ItemDatabase.ItemByIdentifier<Item>(entry.Key);
            AddItem(item, Convert.ToInt32(entry.Value));
        }
    }

    #endregion

    #region Events


    #endregion
}
