using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "DungeonCrawler/Weapon")]
public class Weapon : Item
{
    public int Damage;
    public float Speed; // AttacksPerSecond
    public AttackShapeMarker[] AttackShape;
    public Skill Skill;

    public override void DeserializeFromJson(string json)
    {
        base.DeserializeFromJson(json);
        Dictionary<string, object> data = SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json);
        Damage = Convert.ToInt32(data["Damage"]);
        Speed = Convert.ToInt32(data["Speed"]);

        List<AttackShapeMarker> attackShapes = new List<AttackShapeMarker>();
        foreach (Dictionary<string, object> shapeData in SerializationUtilitites.DeserializeFromObject<Dictionary<string, object>[]>(data["AttackShape"]))
        {
            AttackShapeMarker attackShape = new AttackShapeMarker();
            attackShape.DeserializeFromData(shapeData);
            attackShapes.Add(attackShape);
        }
        AttackShape = attackShapes.ToArray();
    }
}