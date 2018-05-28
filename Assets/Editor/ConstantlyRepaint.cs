using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class ConstantlyRepaint : Editor 
{
    public override void OnInspectorGUI()
    {
        Transform t = (Transform)target;

        t.localPosition = EditorGUILayout.Vector3Field("Position", t.localPosition);
        t.localEulerAngles = EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
        t.localScale = EditorGUILayout.Vector3Field("Scale", t.localScale);
    }

    private void OnSceneGUI()
    {
        SceneView.RepaintAll();
    }
}
