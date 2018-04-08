#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Worldmap.Worldmap), true)]
public class WorldmapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Worldmap.Worldmap worldmap = target as Worldmap.Worldmap;

        if (GUILayout.Button(string.Format("Initialize ({0})", worldmap.PlayerSettings.CurrentLocation)))
        {    
            worldmap.Initialize(worldmap.PlayerSettings.CurrentLocation);
        }
    }
}

#endif