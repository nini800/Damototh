using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class EditorUtilities : MonoBehaviour {


    /*[MenuItem("Edit/Custom Shortcuts/Delete animators _#F1")]
    static void MergeModels()
    {
        MeshFilter f = Selection.gameObjects[0].GetComponent<MeshFilter>();

        if (f.transform.localScale.x != 1 || f.transform.localScale.y != 1 || f.transform.localScale.z != 1)
        {
            List<Vector3> vertices = new List<Vector3>(f.sharedMesh.vertices);
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = f.transform.TransformVector(vertices[i]);
            }
            f.transform.localScale = Vector3.one;
            for (int i = 0; i < vertices.Count; i++)
            {
                vertices[i] = f.transform.InverseTransformVector(vertices[i]);
            }
            f.sharedMesh.vertices = vertices.ToArray();
        }

        foreach (MeshFilter filter in FindObjectsOfType<MeshFilter>())
        {
            if (filter == f)
                continue;
            if (filter.sharedMesh != f.sharedMesh)
                continue;
            if (filter.transform.localScale != Vector3.one)
                filter.transform.localScale = Vector3.one;
        }
    }*/

    /*[MenuItem("Edit/Custom Shortcuts/Delete animators _#F1")]
	static void MergeModels()
	{
        foreach (Animator item in FindObjectsOfType<Animator>())
        {
            DestroyImmediate(item);
        }
    }*/

    /*[MenuItem("Edit/Custom Shortcuts/SpawnPillar _o")]
    static void SpawnPillar()
    {
        Instantiate(GameObject.Find("Pillar"), new Vector3(Random.Range(-500f, 500f), 7f, Random.Range(-500f, 500f)), Quaternion.Euler(0f, Random.Range(0f, 360f), 0f), GameObject.Find("Terrain").transform);
	}*/


    /*[MenuItem("Edit/Custom Shortcuts/SpawnTerrain _o")]
     static void SpawnTerrain()
     {
         for(int i = -3; i < 4; i++)
         {
             for (int j = -3; j < 4; j++)
             {
                 if (i == 0 && j == 0)
                     continue;

                 Instantiate(GameObject.Find("Terrain"), new Vector3(i * 1000, 0f, j *1000), Quaternion.identity, GameObject.Find("Map").transform);
             }
         }

     }*/

    /*[MenuItem("GameObject/TransformCubeToFaces _#F1")]
    static void TransformCubeToFaces()
    {
        GameObject[] gos;
        gos = Selection.gameObjects;

        for (int i = 0; i < gos.Length; i++)
        {
            for (int x = 0; x < gos[i].transform.localScale.x; x++)
            {
                for (int y = 0; y < gos[i].transform.localScale.y; y++)
                {
                    for (int z = 0; z < gos[i].transform.localScale.z; z++)
                    {
                        TrySpawnSquadOnEachSide(new Vector3(CalculatePosWithSize(x, Mathf.RoundToInt(gos[i].transform.localScale.x)),
                                                            CalculatePosWithSize(y, Mathf.RoundToInt(gos[i].transform.localScale.y)),
                                                            CalculatePosWithSize(z, Mathf.RoundToInt(gos[i].transform.localScale.z))) + gos[i].transform.position, gos);
                    }
                }
            }    


        }
    }
    static float CalculatePosWithSize(int pos, int max)
    {
        return pos - (max - 1) / 2;
    }

    static void TrySpawnSquadOnEachSide(Vector3 pos, GameObject[] gos)
    {
        //Debug.Log("TrySpawn each squad " + pos);
        TrySpawnSquad(pos, new Vector3(1, 0, 0), gos);
        TrySpawnSquad(pos, new Vector3(-1, 0, 0), gos);
        TrySpawnSquad(pos, new Vector3(0, 1, 0), gos);
        TrySpawnSquad(pos, new Vector3(0, -1, 0), gos);
        TrySpawnSquad(pos, new Vector3(0, 0, 1), gos);
        TrySpawnSquad(pos, new Vector3(0, 0, -1), gos);
    }
    static void TrySpawnSquad(Vector3 pos, Vector3 offset, GameObject[] gos)
    {
        pos += offset;
        bool canSpawn = true;
        for (int i = 0; i < gos.Length; i++)
        {
            if ((CompareTwoAxis(pos.x, gos[i].transform.position.x, gos[i].transform.localScale.x) &&
                  CompareTwoAxis(pos.y, gos[i].transform.position.y, gos[i].transform.localScale.y) &&
                  CompareTwoAxis(pos.z, gos[i].transform.position.z, gos[i].transform.localScale.z)))
            {
                //Debug.Log("Cant spawn here " + pos);
                canSpawn = false;
                break;
            }
        }
        if (canSpawn)
        {
            //Debug.Log("Yes it can spawn here " + pos + " ");
            Transform temp = ((GameObject)Instantiate(Resources.Load("QuadBasis"), pos - offset / 2f, Quaternion.identity)).transform;
            if (offset.x == 1)
                temp.localEulerAngles = new Vector3(0, -90, 0);
            if (offset.x == -1)
                temp.localEulerAngles = new Vector3(0, 90, 0);
            if (offset.y == 1)
                temp.localEulerAngles = new Vector3(90, 0, 0);
            if (offset.y == -1)
                temp.localEulerAngles = new Vector3(-90, 0, 0);
            if (offset.z == 1)
                temp.localEulerAngles = new Vector3(0, 180, 0);
        }
    }
    static bool CompareTwoAxis(float one, float two, float scale)
    {
        //Debug.Log(one + " / " + two + " / " + scale);
       return (one >= two - scale / 2f && one <= two + scale / 2f);
    }*/


    /*// used to create one single 3D model on simples items
    [MenuItem("GameObject/Test _#F2")]
    static void Test()
    {
        GameObject[] gos;
        gos = Selection.gameObjects;

        Vector3 pos = Vector3.zero;
        Mesh mesh = new Mesh();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector2> uv = new List<Vector2>();
        

        for (int i = 0; i < gos.Length; i++)
        {
            pos += gos[i].transform.position;
        }
        pos /= gos.Length;

        int offset = 0, count = 0;
        for (int i = 0; i < gos.Length; i++)
        {
            foreach (Vector3 vert in gos[i].GetComponent<MeshFilter>().sharedMesh.vertices)
            {
                vertices.Add
                (gos[i].transform.TransformVector(new Vector3(vert.x,
                             vert.y,
                             vert.z)) 
                             + gos[i].transform.position - pos);
            }                
            foreach (int tri in gos[i].GetComponent<MeshFilter>().sharedMesh.triangles)
                triangles.Add(tri + offset);
            foreach (Vector2 u in gos[i].GetComponent<MeshFilter>().sharedMesh.uv)
                uv.Add(u);
            foreach (Vector3 n in gos[i].GetComponent<MeshFilter>().sharedMesh.normals)
                normals.Add(n);

            offset += gos[i].GetComponent<MeshFilter>().sharedMesh.vertices.Length;
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateNormals();
        mesh.name = "Instantiated";

        string filePath = EditorUtility.SaveFilePanelInProject
        (
            "Save Mesh",
             mesh.name + ".asset",
            "asset",
            "Choose a file location"
        );

        if (filePath != "")
        {
            AssetDatabase.CreateAsset(mesh, filePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        GameObject temp = new GameObject();
        temp.transform.position = pos;
        temp.transform.parent = GameObject.Find("World").transform;

        temp.AddComponent<MeshRenderer>().sharedMaterial = gos[0].GetComponent<MeshRenderer>().sharedMaterial;
        MeshFilter filter = temp.AddComponent<MeshFilter>();
        temp.AddComponent<MeshCollider>().sharedMesh = mesh;

        filter.sharedMesh = mesh;      
    }*/

}
