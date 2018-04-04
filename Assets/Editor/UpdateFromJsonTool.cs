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
        string itemsDir = Path.Combine(Path.Combine(jsonRootDir, "Items"), "Items");
        foreach (string file in Directory.GetFiles(itemsDir, "*.json"))
        {
            string json = File.ReadAllText(file);
            string name = new FileInfo(file).Name.Split('.')[0];
            string path = string.Format("Assets/ScriptableObjects/Items/Items/{0}.asset", name);

            if (File.Exists(path))
            {
                Item item = (Item)AssetDatabase.LoadAssetAtPath(path, typeof(Item));
                item.DeserializeFromJson(json);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Item item = ScriptableObject.CreateInstance<Item>();
                item.DeserializeFromJson(json);
                SaveAsset(item, path);
            }
        }
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