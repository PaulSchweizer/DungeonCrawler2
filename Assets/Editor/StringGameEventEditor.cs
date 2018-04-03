#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StringGameEvent), true)]
public class StringGameEventEditor : Editor
{
    public string Data;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Data = EditorGUILayout.TextField("");

        if (GUILayout.Button("Raise"))
        {
            StringGameEvent gameEvent = target as StringGameEvent;
            gameEvent.Raise(Data);
        }
    }
}

#endif