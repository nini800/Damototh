using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardPathNode : MonoBehaviour
{
    public Vector3 Position { get { return transform.position; } }
    private void OnDrawGizmos()
    {
        Gizmos.color = GetComponentInParent<GuardPath>().PathColor;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f + Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f + Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f - Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f - Vector3.forward * 0.25f);

        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f + Vector3.forward * 0.25f, transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f + Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f - Vector3.forward * 0.25f, transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f + Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f - Vector3.forward * 0.25f, transform.position + Vector3.up * 0.5f - Vector3.right * 0.25f - Vector3.forward * 0.25f);
        Gizmos.DrawLine(transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f + Vector3.forward * 0.25f, transform.position + Vector3.up * 0.5f + Vector3.right * 0.25f - Vector3.forward * 0.25f);
    }
}
