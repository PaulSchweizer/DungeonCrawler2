﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

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

public class Inventory : MonoBehaviour
{
    public List<InventoryEntry> Items = new List<InventoryEntry>();

    public void AddItem(Item item, int amount = 1)
    {
        for(int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item.Id == item.Id)
            {
                Items[i].Amount += amount;
                return;
            }
        }
        Items.Add(new InventoryEntry(item, amount));
    }

    public void RemoveItem(Item item, int amount = 1)
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (Items[i].Item.Id == item.Id)
            {
                Items[i].Amount -= amount;
                if (Items[i].Amount <= 0)
                {
                    Items.Remove(Items[i]);
                }
                return;
            }
        }
    }

    public int Amount(Item item)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item.Id == item.Id)
            {
                return Items[i].Amount;
            }
        }
        return 0;
    }

    public void Clear()
    {
        Items.Clear();
    }

    public static Inventory operator +(Inventory thisInventory, Inventory thatInventory)
    {
        foreach (InventoryEntry entry in thatInventory.Items)
        {
            thisInventory.AddItem(entry.Item, thatInventory.Amount(entry.Item));
        }
        thatInventory.Clear();
        return thisInventory;
    }
}