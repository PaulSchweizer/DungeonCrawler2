using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

[CreateAssetMenu(fileName = "Aspect", menuName = "DungeonCrawler/Aspect")]
public class Aspect : ScriptableObject
{
    public string Name;
    public int Bonus;
    public string[] Tags;
    public Skill[] Skills;

    [JsonIgnore]
    public int Cost
    {
        get
        {
            int cost = Tags.Length + Skills.Length;
            if (Bonus > 0)
            {
                cost += 2;
            }
            else if (Bonus < 0)
            {
                cost = -cost - 2;
            }
            return cost;
        }
    }

    public int Matches(string[] tags)
    {
        int matches = 0;

        if (Array.Exists(Tags, element => element == "any"))
        {
            matches += 1;
        }

        foreach (string tag in tags)
        {
            if (Array.Exists(Tags, element => element == tag.ToLower()))
            {
                matches += 1;
            }
            //else
            //{
            //    foreach (string synonym in Rulebook.SynonymsOf(tag))
            //    {
            //        if (Array.Exists(Tags, element => element == synonym.ToLower()))
            //        {
            //            matches += 1;
            //            break;
            //        }
            //    }
            //}
        }
        return matches;
    }
}