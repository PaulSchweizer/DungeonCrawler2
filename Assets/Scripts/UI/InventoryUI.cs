using UnityEngine;
using System.Collections.Generic;
using SlotSystem;

public class InventoryUI : MonoBehaviour
{
    public Stats Stats;
    public Inventory Inventory;
    public SlotView SlotView;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Inventory != null)
        {
            SlotView.InitFromInventoryItems(Inventory, Stats);
            Inventory.OnItemAdded += ItemAddedToInventory;
            Inventory.OnItemRemoved += ItemRemovedFromInventory;
        }
    }

    private void OnDisable()
    {
        if (Inventory != null)
        {
            Inventory.OnItemAdded -= ItemAddedToInventory;
            Inventory.OnItemRemoved -= ItemRemovedFromInventory;
        }
    }

    public void ItemAddedToInventory(Inventory sender, ItemEventArgs e)
    {
        SlotView.AddItem(e.Item, e.Amount);
    }

    public void ItemRemovedFromInventory(Inventory sender, ItemEventArgs e)
    {
        SlotView.RemoveItem(e.Item, e.Amount);
    }
}