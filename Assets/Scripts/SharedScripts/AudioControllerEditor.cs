using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioController))]
public class AudioControllerEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        GUILayout.Label("Quick Audio Adjust", GUILayout.Width(150));
        GUILayout.Width(200);
        if (GUILayout.Button("Decrease %25"))
            ((AudioController)target).DecreaseAudio(25);
        if (GUILayout.Button("Increase %25"))
            ((AudioController)target).IncreaseAudio(25);
        GUILayout.EndHorizontal();
        serializedObject.ApplyModifiedProperties();
    }
}