using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX : MonoBehaviour 
{
    
    public static void SpawnHitFX(Vector3 pos, Vector3 dir, Transform parent)
    {
        Instantiate(Resources.Load<GameObject>("FX\\EnemyHitFX"), pos, Quaternion.identity, parent).transform.LookAt(pos + dir);
    }
	
}