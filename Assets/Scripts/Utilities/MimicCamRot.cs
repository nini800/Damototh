using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicCamRot : MonoBehaviour 
{
    public Transform mainCam;   
    protected void Start ()
    {
        if (mainCam == null)
            mainCam = Camera.main.transform;
	}
	
	protected void Update ()
    {
        transform.LookAt(transform.position - mainCam.transform.forward);
	}
	
}