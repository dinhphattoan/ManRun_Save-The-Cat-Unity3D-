#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GameMechanic;

[CustomEditor(typeof(GameManager))]
public class GameManager_editor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GUILayout.Space(15);
        if(GUILayout.Button("Restart Round", GUILayout.Height(30)))
        {
            ((GameManager)target).ReloadLevel();
        }
        if(GUILayout.Button("Main menu", GUILayout.Height(30)))
        {
            ((GameManager)target).HandleMainMenu();
        }
    }
}
#endif