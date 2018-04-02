using UnityEngine;
using System.Collections.Generic;

namespace SlotSystem
{
    public class SlotView : MonoBehaviour
    {
        [Header("Settings")]
        public int NumberOfSlots;
        public bool InfiniteSlots;

        [Header("Prefabs")]
        public Slot SlotPrefab;
        public Transform SlotParent;
        public SlottableItem SlottableItemPrefab;

        public Dictionary<string, SlottableItem> Items = new Dictionary<string, SlottableItem>() { };
        private readonly List<Slot> _slots = new List<Slot>();

        private void Awake()
        {
            foreach (Slot slot in SlotParent.GetComponentsInChildren<Slot>())
            {
                _slots.Add(slot);
            }

            for (int i = 0; i < NumberOfSlots - (_slots.Count - 1); i++)
            {
                AddSlot();
            }
        }

        public void InitFromInventoryItems(Inventory inventory, Stats stats = null)
        {
            ResetSlots();
            Items.Clear();
            if (inventory.Entries.Count > NumberOfSlots)
            {
                NumberOfSlots = inventory.Entries.Count;
            }

            List<Item> items = new List<Item>();
            if (stats != null)
            {
                items = stats.EquippedItems;
            }

            foreach (InventoryEntry entry in inventory.Entries)
            {
                if (!items.Contains(entry.Item))
                {
                    AddItem(entry.Item, entry.Amount);
                }
            }
        }

        public void RemoveItem(Item item, int amount)
        {
            SlottableItem viewItem;
            if (Items.TryGetValue(item.Name, out viewItem))
            {
                viewItem.Amount -= amount;
                if (viewItem.Amount <= 0)
                {
                    Items.Remove(item.Name);
                    viewItem.Slot.Clear();
                }
                else
                {
                    viewItem.UpdateDisplay();
                }
            }
        }

        public void ResetSlots()
        {
            foreach (Slot slot in _slots)
            {
                if (slot.Item != null)
                {
                    slot.Item.gameObject.SetActive(false);
                    Items.Remove(slot.Item.Name.text);
                    slot.Item.Amount = 0;
                    slot.Item.Item = null;
                    SlottableItem item = slot.Item;
                    slot.Clear();
                    Slot target = NextAvailableSlot();
                    if (target != null)
                    {
                        target.Drop(item);
                    }
                }
            }
        }

        protected Slot AddSlot()
        {
            Slot slot = Instantiate(SlotPrefab, SlotParent);
            slot.name = string.Format("Slot{0}", _slots.Count);
            _slots.Add(slot);
            slot.transform.localScale = new Vector3(1, 1, 1);
            slot.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            return slot;
        }

        public Slot NextAvailableSlot()
        {
            if (Items.Count > NumberOfSlots)
            {
                return InfiniteSlots ? AddSlot() : null;
            }
            foreach (Slot slot in _slots)
            {
                if (slot.Item == null)
                {
                    return slot;
                }
            }
            return InfiniteSlots ? AddSlot() : null;
        }

        public Slot SlotByName(string name)
        {
            foreach (Slot slot in _slots)
            {
                if (slot.name == name)
                {
                    return slot;
                }
            }
            return null;
        }

        public void AddItem(SlottableItem item, string slotName = null)
        {
            Slot slot = null;
            if (slotName == null)
            {
                slot = NextAvailableSlot();
            }
            else
            {
                slot = SlotByName(slotName);
            }
            if (slot != null)
            {
                slot.Drop(item);
            }
        }

        public void AddItem(Item item, int amount, string slotName = null, bool updateOnly = false)
        {
            SlottableItem viewItem;
            if (Items.TryGetValue(item.Name, out viewItem))
            {
                if (updateOnly) viewItem.Amount = amount;
                else viewItem.Amount += amount;
                viewItem.UpdateDisplay();
            }
            else
            {
                foreach (Slot slot in _slots)
                {
                    if (slot.Item != null)
                    {
                        if (slot.Item.Item == null || !slot.Item.gameObject.activeSelf)
                        {
                            slot.Item.Init(item, amount);
                            return;
                        }
                    }
                }
                viewItem = Instantiate(SlottableItemPrefab);
                viewItem.Init(item, amount);
                Items[item.Name] = viewItem;
                AddItem(viewItem, slotName);
            }
        }
    }
}