using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "DungeonCrawler/Weapon")]
public class Weapon : Item
{
    public int Damage;
    public float Speed; // AttacksPerSecond
    public AttackShapeMarker[] AttackShape;
    public Skill Skill;
}