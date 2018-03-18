using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Lootable : MonoBehaviour
{
    public Inventory Inventory;

    public void OnEnable()
    {
        if (Inventory != null)
        {
            Inventory = ScriptableObject.Instantiate<Inventory>(Inventory);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Inventory != null)
        {
            PlayerCharacter pc = other.GetComponent<PlayerCharacter>();
            pc.Inventory += Inventory;
            gameObject.SetActive(false);
        }
    }
}

