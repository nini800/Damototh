using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringArm : MonoBehaviour 
{
    [SerializeField] LayerMask whatIsCollider;
    [SerializeField] Vector3 offset;
    [SerializeField] float normalFactor = 0.05f, directionFactor = 0.05f;

    protected void Start ()
    {
        offset = transform.localPosition;
	}
	
	protected void Update ()
    {
        RayHandler();
	}

    RaycastHit hit;
    Ray ray;
    void RayHandler()
    {
        hit = new RaycastHit();
        ray = new Ray(transform.parent.position, transform.parent.TransformDirection(offset.normalized));
        
        Physics.Raycast(ray, out hit, offset.magnitude, whatIsCollider);

        if (hit.collider != null)
            transform.position = hit.point - ray.direction * directionFactor + hit.normal * normalFactor;
        else
            transform.localPosition = offset;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.parent.position, transform.position);

        Gizmos.color = Color.red;
        if (hit.collider != null)
            Gizmos.DrawWireSphere(hit.point, 0.1f);
        Gizmos.DrawLine(transform.parent.position, transform.parent.position + transform.parent.TransformDirection(offset.normalized) * offset.magnitude);
    }

}