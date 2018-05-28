using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardPath : MonoBehaviour {

    [SerializeField] bool loop = false;
    [SerializeField] Color pathColor;
    [SerializeField] Color lineColor;
    public Color PathColor { get { return pathColor; } }

    GuardPathNode[] nodes;

    private void Awake()
    {
        nodes = new GuardPathNode[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            nodes[i] = transform.GetChild(i).GetComponent<GuardPathNode>();
        }
    }

    public Vector3 GetNodePosition(int number)
    {
        if (nodes == null || nodes.Length == 0)
            Awake();

        if (loop)
        {
            return nodes[number % nodes.Length].Position;
        }
        else
        {
            //First cycle is from start to end, then, we must skip end, start, end, start ...
            if (number < nodes.Length)
                return nodes[number].Position;
            else
            {
                //Here we are above nodes.length
                //First we put number back to a 0 basis
                number -= nodes.Length;

                //Then we consider a length of 3 nodes possible as the first ones are always skipped when going back
                bool even = false;
                while (number >= nodes.Length - 1)
                {
                    //Each time we revert the direction
                    even = !even;
                    number -= nodes.Length - 1;
                }

                //Calculate the node depending on even's telling direction !
                if (even)
                    return nodes[number + 1].Position;
                else
                    return nodes[(nodes.Length - 1) - (number + 1)].Position;
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        for (int i = 0; i < (loop ? transform.childCount : (transform.childCount - 1)); i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild((i + 1) % transform.childCount).position);
        }
    }
}
