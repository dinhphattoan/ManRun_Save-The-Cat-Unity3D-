#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralLevel))]
public class LevelGenerator_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var target = (ProceduralLevel)serializedObject.targetObject;
        if (GUILayout.Button("Generate 10 Level", GUILayout.Height(30)))
        {
            target.Initialize(10);
        }
        if (GUILayout.Button("Apply Level Changes", GUILayout.Height(30)))
        {
            target.Reinitialize();
        }
    }
}
#endif