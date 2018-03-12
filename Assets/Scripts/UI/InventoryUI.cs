using UnityEngine;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public Inventory Inventory;
    public SlotSystem.SlotView SlotView;

    public void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (Inventory != null)
        {
            SlotView.InitFromInventoryItems(Inventory);
        }
    }
}