using UnityEngine;
using UnityEditor;
using System.IO;

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
        UpdateItems();
        // TODO: Weapons
        // TODO: Armour
        // TODO: Enemies
        // TODO: Quests
    }

    private void UpdateItems()
    {
        string aspectDatabasePath = string.Format("Assets/ScriptableObjects/Aspects/AspectDatabase.asset", name);
        AspectDatabase aspectDatabase = (AspectDatabase)AssetDatabase.LoadAssetAtPath(aspectDatabasePath, typeof(AspectDatabase));
        string itemsDir = Path.Combine(Path.Combine(jsonRootDir, "Items"), "Items");
        foreach (string file in Directory.GetFiles(itemsDir, "*.json"))
        {
            string json = File.ReadAllText(file);
            string name = new FileInfo(file).Name.Split('.')[0];
            string path = string.Format("Assets/ScriptableObjects/Items/Items/{0}.asset", name);
            Item item = null;
            if (File.Exists(path))
            {
                item = (Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item));
            }
            else
            {
                item = ScriptableObject.CreateInstance<Item>();
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
            string spritePath = string.Format("Assets/Sprites/Items/Items/{0}.asset", name);
            if (File.Exists(spritePath))
            {
                Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(spritePath, typeof(Sprite));
                item.Sprite = sprite;
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