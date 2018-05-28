using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraQuickController : MonoBehaviour {

    [SerializeField] float speed = 0.1f;

	// Use this for initialization
	void Start () {
		
	}


    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
            Cursor.lockState = CursorLockMode.Locked;
        else if (Input.GetMouseButtonUp(1))
            Cursor.lockState = CursorLockMode.None;
    }

    void FixedUpdate () 
    {
        float speed = this.speed * (Input.GetKey(KeyCode.LeftShift) ? 2f : 1f);

        if (Input.GetKey(KeyCode.Z))
            transform.position += transform.forward * speed;
        if (Input.GetKey(KeyCode.S))
            transform.position -= transform.forward * speed;
        if (Input.GetKey(KeyCode.Q))
            transform.position -= transform.right * speed;
        if (Input.GetKey(KeyCode.D))
            transform.position += transform.right * speed;
        if (Input.GetKey(KeyCode.E))
            transform.position += transform.up * speed;
        if (Input.GetKey(KeyCode.A))
            transform.position -= transform.up * speed;




        if (Cursor.lockState == CursorLockMode.Locked)
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + -Input.GetAxis("Mouse Y"), transform.eulerAngles.y + Input.GetAxis("Mouse X"), transform.eulerAngles.z);
    }
}
