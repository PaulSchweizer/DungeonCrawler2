using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "DungeonCrawler/Weapon")]
public class Weapon : Item
{
    public int Damage;
    public float Speed;
}