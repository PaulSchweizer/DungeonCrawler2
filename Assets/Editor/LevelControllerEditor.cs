#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelController), true)]
public class LevelControllerEditor : Editor
{
    public string PreviousSceneName;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PreviousSceneName = EditorGUILayout.TextField("");
        if (GUILayout.Button("Initialize"))
        {
            LevelController levelController = target as LevelController;
            levelController.Initialize(PreviousSceneName);
        }
    }
}

#endif