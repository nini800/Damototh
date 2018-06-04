using UnityEngine;
using UnityEditor;
using System.Collections;

public class CustomShortcuts : MonoBehaviour {

    [MenuItem("Edit/Custom Shortcuts/Input _#%a")]
    static void InputShortcut()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Input");
    }
    [MenuItem("Edit/Custom Shortcuts/Player _#%z")]
    static void PlayerShortcut()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
    }
    [MenuItem("Edit/Custom Shortcuts/Physics _#%e")]
    static void PhysicsShortcut()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Physics");
    }
    [MenuItem("Edit/Custom Shortcuts/Quality _#%q")]
    static void QualityShortcut()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Quality");
    }
    [MenuItem("Edit/Custom Shortcuts/Graphics _#%t")]
    static void GraphicsShortcut()
    {
        EditorApplication.ExecuteMenuItem("Edit/Project Settings/Graphics");
    }

    [MenuItem("Edit/Custom Shortcuts/DoubleSided _#%r")]
    static void DoubleSided()
    {
        foreach (MeshRenderer rend in Selection.gameObjects[0].GetComponentsInChildren<MeshRenderer>())
        {
            rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;
        }
    }
}
