using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

        public Dictionary<string, SlottableItem> _items = new Dictionary<string, SlottableItem>() { };
        private readonly List<Slot> _slots = new List<Slot>();

        private void Awake()
        {
            for (int i = 0; i < NumberOfSlots; i++)
            {
                AddSlot();
            }
        }

        public void InitFromInventoryItems(Inventory inventory)
        {
            ResetSlots();
            _items.Clear();
            if (inventory.Entries.Count > NumberOfSlots)
            {
                NumberOfSlots = inventory.Entries.Count;
            }
            foreach (InventoryEntry entry in inventory.Entries)
            {
                AddItem(entry.Item, entry.Amount);
            }
        }

        public void RemoveItem(Item item, int amount)
        {
            SlottableItem viewItem;
            if (_items.TryGetValue(item.Name, out viewItem))
            {
                viewItem.Amount -= amount;
                if (viewItem.Amount <= 0)
                {
                    _items.Remove(item.Name);
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
                    _items.Remove(slot.Item.Name.text);
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
            if (_items.Count > NumberOfSlots)
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

        public void AddItem(SlottableItem item)
        {
            Slot slot = NextAvailableSlot();
            if (slot != null)
            {
                slot.Drop(item);
            }
        }

        public void AddItem(Item item, int amount)
        {
            SlottableItem viewItem;
            if (_items.TryGetValue(item.Name, out viewItem))
            {
                viewItem.Amount += amount;
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
                _items[item.Name] = viewItem;
                AddItem(viewItem);
            }
        }
    }
}