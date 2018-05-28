using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class GizmosDisplayer : MonoBehaviour {
    #if UNITY_EDITOR
    [SerializeField] Vector3 size;
    [SerializeField] Color color;
    [SerializeField] GUIStyle style;

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, size);
    }

    private void OnDrawGizmosSelected()
    {
        Handles.Label(transform.position, gameObject.name, style);
    }
    #endif
}
