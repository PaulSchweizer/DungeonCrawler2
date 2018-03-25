using UnityEngine;
using SlotSystem;
using UnityEngine.UI;

public class EuipmentUI : MonoBehaviour, ISlotChanged
{
    public Stats Stats;
    public Inventory Inventory;
    public SlotView SlotView;

    public Text DamageValue;
    public Text ProtectionValue;
    public Text AttackRangeValue;

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        foreach (EquipmentSlot equipmentSlot in Stats.Equipment)
        {
            if (equipmentSlot.Item != null)
            {
                SlotView.AddItem(equipmentSlot.Item, 
                                    Inventory.Amount(equipmentSlot.Item), 
                                    equipmentSlot.Name, 
                                    updateOnly: true);
            }
        }
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        // Combat
        DamageValue.text = Stats.Damage.ToString();
        ProtectionValue.text = Stats.Protection.ToString();
    }

    public void SlotChanged(Slot slot, SlottableItem newItem, SlottableItem oldItem)
    {
        EquipmentSlot equipmentSlot = Stats.EquipmentSlotByName(slot.name);
        if (oldItem != null) Stats.UnEquip(oldItem.Item, equipmentSlot);
        if (newItem != null) Stats.Equip(newItem.Item, equipmentSlot);
    }
}
