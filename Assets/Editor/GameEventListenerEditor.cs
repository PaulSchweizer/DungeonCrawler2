#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEventListener), true)]
public class GameEventListenerEditor : Editor
{
    public string Notes;

    public override void OnInspectorGUI()
    {
        Notes = EditorGUILayout.TextField("");
        DrawDefaultInspector();
    }
}

#endif