using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HelicopterController))]
public class HelicopterControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (Application.isPlaying && GUILayout.Button("Destroy"))
            ((HelicopterController)target).Explode();
    }
}