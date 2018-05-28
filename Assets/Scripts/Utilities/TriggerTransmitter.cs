using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerTransmitter : MonoBehaviour {

    [SerializeField] int parentNumber = 1;

	void OnTriggerEnter(Collider coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnTriggerEnter", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit(Collider coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnTriggerExit", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnCollisionEnter(Collision coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnCollisionEnter", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnCollisionExit(Collision coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnCollisionExit", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnTriggerEnter2D", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnTriggerExit2D", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnCollisionEnter2D", coll, SendMessageOptions.DontRequireReceiver);
    }

    void OnCollisionExit2D(Collision2D coll)
    {
        Transform parent = transform.parent;
        for (int i = 1; i < parentNumber; i++)
            parent = parent.parent;

        parent.SendMessage("OnCollisionExit2D", coll, SendMessageOptions.DontRequireReceiver);
    }
}
