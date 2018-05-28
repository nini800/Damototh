using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PivotChangeWindow : EditorWindow 
{
    bool pivotFreeMove = false;
    Vector3 startPos = Vector3.zero;
    Vector3 startRot = Vector3.zero;

    GameObject o;

    // Add menu named "My Window" to the Window menu
    [MenuItem("Window/Pivot Changer")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        PivotChangeWindow window = (PivotChangeWindow)EditorWindow.GetWindow(typeof(PivotChangeWindow));
        window.minSize = new Vector2(200f, 100);
        window.maxSize = new Vector2(200f, 100);
        window.Show();
    }

    void OnGUI()
    {
        GameObject[] selects = Selection.gameObjects;
        MeshFilter rend = null;
        bool toReturn = false;
        if (selects.Length == 1)
        {
            o = selects[0];

            if (!(rend = o.GetComponent<MeshFilter>()))
            {
                toReturn = true;
            }
        }
        else
            toReturn = true;

        List<Vector3> vertices = !toReturn ? new List<Vector3>(rend.sharedMesh.vertices) : null;

        if (GUILayout.Button("Center Pivot"))
        {
            if (toReturn)
                return;
            pivotFreeMove = false;
            Vector3 middlePos = Vector3.zero;
            for (int i = 0; i < vertices.Count; i++)
            {
                middlePos += vertices[i];
            }
            middlePos /= vertices.Count;

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] -= middlePos;
            }
            o.transform.position += new Vector3(middlePos.x * o.transform.localScale.x, middlePos.y * o.transform.localScale.y, middlePos.z * o.transform.localScale.z);

            rend.sharedMesh.vertices = vertices.ToArray();
                EditorUtility.SetDirty(o);
        }
        else if (GUILayout.Button("Bottom Pivot"))
        {
            if (toReturn)
                return;
            pivotFreeMove = false;
            int bottomVerticeIndex = 0;

            for (int i = 0; i < vertices.Count; i++)
            {
                if (vertices[i].y < vertices[bottomVerticeIndex].y)
                    bottomVerticeIndex = i;
            }

            float yAmount = vertices[bottomVerticeIndex].y;

            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = new Vector3(vertices[i].x, vertices[i].y - yAmount, vertices[i].z);
            }
            o.transform.position += new Vector3(0, yAmount * o.transform.localScale.y, 0);

            rend.sharedMesh.vertices = vertices.ToArray();
                EditorUtility.SetDirty(o);
        }

        if (!pivotFreeMove)
        {
            if (GUILayout.Button("Free Move"))
            {
                if (toReturn)
                    return;
                pivotFreeMove = true;

                startPos = o.transform.position;
                startRot = o.transform.localEulerAngles;
            }
        }
        else
        {
            if (GUILayout.Button("Stop"))
            {
                if (toReturn)
                    return;
                pivotFreeMove = false;
            }
        }

        EditorGUILayout.Separator();
        if (GUILayout.Button("Save Mesh", EditorStyles.miniButton))
        {
            if (toReturn)
                return;

            string path = AssetDatabase.GetAssetPath(rend.sharedMesh.GetInstanceID());
            Mesh mesh = Instantiate<Mesh>(rend.sharedMesh);

            Debug.Log(path);
            if (path.Substring(path.Length - 3, 3).ToUpper() == ("FBX") || path.Substring(path.Length - 3, 3).ToUpper() == ("OBJ"))
            {
                path = path.Substring(0, path.Length - 4);
                path += ".asset";
            }
            else if (path.Substring(path.Length - 6, 6).ToUpper() != (".ASSET"))
                path += ".asset";
            Debug.Log(path);

            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(mesh, path);

            foreach (MeshFilter filt in FindObjectsOfType<MeshFilter>())
            {
                if (filt == rend)
                    continue;
                if (filt.sharedMesh == rend.sharedMesh)
                    filt.sharedMesh = mesh;
            }

            rend.sharedMesh = mesh;
        }
        
    }

    private void Update()
    {
        MeshFilter rend;

        if (o == null)
        {
            GameObject[] selects = Selection.gameObjects;

            if (selects.Length == 1)
                o = selects[0];
            else
                return;
        }
        if (!(rend = o.GetComponent<MeshFilter>()))
            return;


        List<Vector3> vertices = null;
        if (rend != null && rend.sharedMesh != null)
            vertices = new List<Vector3>(rend.sharedMesh.vertices);

        if (pivotFreeMove)
        {
            Tools.pivotMode = PivotMode.Pivot;
            if (o.transform.position != startPos)
            {
                Vector3 diff = o.transform.InverseTransformVector(startPos - o.transform.position);
                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] += (new Vector3(diff.x, diff.y, diff.z));
                }
                startPos = o.transform.position;
                rend.sharedMesh.vertices = vertices.ToArray();
                EditorUtility.SetDirty(o);
            }

            if (o.transform.localEulerAngles != startRot)
            {
                Vector3 temp = o.transform.localEulerAngles;
                o.transform.localEulerAngles = startRot;

                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = o.transform.TransformVector(vertices[i]);
                }

                o.transform.localEulerAngles = temp;
                for (int i = 0; i < vertices.Count; i++)
                {
                    vertices[i] = o.transform.InverseTransformVector(vertices[i]);
                }

                startRot = o.transform.localEulerAngles;
                rend.sharedMesh.vertices = vertices.ToArray();
                rend.sharedMesh.RecalculateNormals();

                EditorUtility.SetDirty(o);
            }
        }
    }
}