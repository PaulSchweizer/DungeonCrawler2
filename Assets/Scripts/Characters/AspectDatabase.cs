using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "AspectDatabase", menuName = "DungeonCrawler/AspectDatabase")]
public class AspectDatabase : ScriptableObject
{
    public Aspect[] Aspects;

    public Aspect AspectByName(string name)
    {
        for (int i = 0; i < Aspects.Length; i++)
        {
            if (Aspects[i].Name == name)
            {
                return Aspects[i];
            }
        }
        return null;
    }
}