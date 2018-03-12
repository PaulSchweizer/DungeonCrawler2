#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InventoryUI), true)]
public class InventoryUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Initialize"))
        {
            InventoryUI inventoryUI = target as InventoryUI;
            inventoryUI.Initialize();
        }
    }
}

#endif