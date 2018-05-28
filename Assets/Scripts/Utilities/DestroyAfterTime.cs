using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FadeType
{
    Light
}
public class DestroyAfterTime : MonoBehaviour {

    [SerializeField] float time = 0.05f;
    [SerializeField] FadeOption[] fadeOptions;

	void Start () {
        foreach (FadeOption fade in fadeOptions)
        {
            switch (fade.type)
            {
                case FadeType.Light:
                    StartCoroutine(LightFade(fade.time));
                    break;
                default:
                    break;
            }
        }

        Destroy(gameObject, time);
	}
	
	void Update () {
		
	}

    IEnumerator LightFade(float time)
    {
        Light light = GetComponentInChildren<Light>();

        if (light != null)
        {
            float baseIntensity = light.intensity;
            float count = 0f;
            while (count <= time)
            {
                light.intensity = baseIntensity * ((time - count) / time);

                yield return new WaitForEndOfFrame();
                count += Time.deltaTime;
            }
        }        
    }
}

[System.Serializable]
public class FadeOption
{
    public FadeType type;
    public float time;
}
