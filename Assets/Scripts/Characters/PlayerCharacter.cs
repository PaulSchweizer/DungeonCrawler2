using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : BaseCharacter
{
    [Header("EquipmentParents")]
    public Transform WeaponParent;
    public Transform ArmourParent;

    private Dictionary<string, GameObject> ItemInstances = new Dictionary<string, GameObject>();

    public override void Awake()
    {
        base.Awake();
    }

    public Transform ParentForEquipmentSlot(EquipmentSlot slot)
    {
        if (slot.Name == "Weapon")
        {
            return WeaponParent;
        }
        else if (slot.Name == "Armour")
        {
            return ArmourParent;
        }
        else
        {
            return transform;
        }
    }

    public void UpdateEquipment()
    {
        foreach (KeyValuePair<string, GameObject> entry in ItemInstances)
        {
            entry.Value.SetActive(false);
        }
        for (int i = 0; i < Stats.Equipment.Length; i++)
        {
            Transform parent = ParentForEquipmentSlot(Stats.Equipment[i]);
            if (Stats.Equipment[i].Item != null)
            {
                if (Stats.Equipment[i].Item.Prefab != null)
                {
                    GameObject instance;
                    if (!ItemInstances.TryGetValue(Stats.Equipment[i].Item.Identifier, out instance))
                    {
                        instance = Instantiate(Stats.Equipment[i].Item.Prefab);
                        ItemInstances[Stats.Equipment[i].Item.Identifier] = instance;
                    }
                    instance.transform.SetParent(parent);
                    instance.transform.localPosition = Vector3.zero;
                    instance.transform.localRotation = Quaternion.identity;
                    instance.transform.localScale = Vector3.one;
                    instance.SetActive(true);
                }
            }
        }
    }
}
