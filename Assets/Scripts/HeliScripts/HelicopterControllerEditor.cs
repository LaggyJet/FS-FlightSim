using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HelicopterController))]
public class HelicopterControllerEditor : Editor {
    public override void OnInspectorGUI() {
        serializedObject.Update();
        base.OnInspectorGUI();
        if (GUILayout.Button("Destroy")){
            ((HelicopterController)target).Explode();
        }
    }
}
