#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SoundController))]
public class SoundController_editor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if(GUILayout.Button("Play Next Audio")) {
            ((SoundController)target).NextMusicClip();
        }
    }
}
#endif