#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEventListener), true)]
public class GameEventListenerEditor : Editor
{
    public string Notes;

    public override void OnInspectorGUI()
    {
        GameEventListener listener = target as GameEventListener;
        if (listener.Event != null) Notes = listener.Event.name;
        Notes = EditorGUILayout.TextField(Notes);
        DrawDefaultInspector();
    }
}

#endif