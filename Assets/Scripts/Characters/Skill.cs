using UnityEngine;

[CreateAssetMenu(fileName = "Skill", menuName = "DungeonCrawler/Skill")]
public class Skill : ScriptableObject
{
    public string Name;
    public string Description;
    public Skill[] OpposingSkills;
    public string[] Actions;
}