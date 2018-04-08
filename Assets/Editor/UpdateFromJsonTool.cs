using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

public class UpdateFromJsonTool : EditorWindow
{
    private string jsonRootDir = "C:/PROJECTS/DungeonCrawler2/Assets/JsonData/GoogleDrive";

    [MenuItem("DungeonCrawler/Update From Json")]
    private static void Init()
    {
        UpdateFromJsonTool window = (UpdateFromJsonTool)EditorWindow.GetWindow(typeof(UpdateFromJsonTool));
        window.Show();
    }
    private void OnGUI()
    {
        jsonRootDir = EditorGUILayout.TextField(jsonRootDir);
        if (GUILayout.Button("Update From Json"))
        {
            UpdateFromJson();
        }
    }
    private void UpdateFromJson()
    {
        // TODO: Aspects
        UpdateItems<Item>();
        UpdateItems<Weapon>();
        UpdateItems<Armour>();
        // TODO: Enemies
        // TODO: Quests
    }

    private void UpdateItems<T>() where T : Item
    {
        string itemType = typeof(T).FullName;
        string aspectDatabasePath = "Assets/ScriptableObjects/Aspects/AspectDatabase.asset";
        AspectDatabase aspectDatabase = (AspectDatabase)AssetDatabase.LoadAssetAtPath(aspectDatabasePath, typeof(AspectDatabase));
        string itemsDir = Path.Combine(Path.Combine(jsonRootDir, "Items"), itemType);
        foreach (string file in Directory.GetFiles(itemsDir, "*.json"))
        {
            string json = File.ReadAllText(file);
            string name = new FileInfo(file).Name.Split('.')[0];
            string path = string.Format("Assets/ScriptableObjects/Items/{0}/{1}.asset", itemType, name);
            T item = default(T);
            if (File.Exists(path))
            {
                item = (T)AssetDatabase.LoadAssetAtPath(path, typeof(T));
            }
            else
            {
                item = ScriptableObject.CreateInstance<T>();
                SaveAsset(item, path);
            }
            item.AspectDatabase = aspectDatabase;
            item.DeserializeFromJson(json);
            string prefabPath = string.Format("Assets/Prefabs/Items/Items/{0}.asset", name);
            if (File.Exists(prefabPath))
            {
                GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath(prefabPath, typeof(GameObject));
                item.Prefab = prefab;
            }
            string[] spriteFiles = Directory.GetFiles(string.Format("Assets/Sprites/Items/{0}", itemType), string.Format("{0}.*", name));
            if (spriteFiles.Length > 0)
            {
                Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(spriteFiles[0], typeof(Sprite));
                item.Sprite = sprite;
            }

            if (itemType == "Weapon")
            {
                string skillDatabasePath = "Assets/ScriptableObjects/Characters/Skills/SkillDatabase.asset";
                SkillDatabase skillDatabase = (SkillDatabase)AssetDatabase.LoadAssetAtPath(skillDatabasePath, typeof(SkillDatabase));
                Weapon weapon = item as Weapon;
                string skill = Convert.ToString(SerializationUtilitites.DeserializeFromJson<Dictionary<string, object>>(json)["Skill"]);
                weapon.Skill = skillDatabase.SkillByName(skill);
            }
        }

        AssetDatabase.SaveAssets();
        // TODO: Update the Database
    }

    private void SaveAsset<T>(T asset, string path) where T : ScriptableObject
    {
        string assetPathAndName = path;
        AssetDatabase.CreateAsset(asset, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}