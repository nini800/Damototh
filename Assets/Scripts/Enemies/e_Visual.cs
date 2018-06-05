using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_Visual : e_Base 
{
    [SerializeField] protected float rotSpeed = 1f;

	protected virtual void Update ()
    {
        Visual.position = Body.position;
        Visual.rotation = Body.rotation;
    }

    protected virtual void OnTakeHit()
    {
        StopCoroutine("TakeHitColorCoroutine");
        StartCoroutine("TakeHitColorCoroutine");
    }

    IEnumerator TakeHitColorCoroutine()
    {
        Material mat = Visual.GetComponentInChildren<Renderer>().material;
        Color oColor = mat.color;
        float count = 0f;
        while (count < 0.2f)
        {
            float progress = count / 0.2f;
            mat.color = oColor + new Color(0.1f * (1-progress), -0.1f * (1 - progress), -0.1f * (1 - progress));
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime;
        }
        mat.color = oColor;
    }
	
}