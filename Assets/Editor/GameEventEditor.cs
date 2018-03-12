#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent), true)]
public class GameEventEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Raise"))
        {
            GameEvent gameEvent = target as GameEvent;
            gameEvent.Raise();
        }
    }
}

#endif