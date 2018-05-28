using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_GuardAI : e_EnemyAI
{
    [SerializeField] GuardPath path;

    int currentPathNode = 0;

    protected override void Behaviour()
    {
        if (targetBody == null && path != null)
        {
            Agent.SetDestination(path.GetNodePosition(currentPathNode));

            if (Vector3.Distance(Body.position, path.GetNodePosition(currentPathNode)) <= Agent.stoppingDistance)
                currentPathNode++;

            CheckAround(targetDistance);
        }
        else
        {
            base.Behaviour();
        }
    }

    private void OnDrawGizmos()
    {
        if (path != null)
            Gizmos.DrawLine(transform.Find("Body").position, path.GetNodePosition(0));
    }
}
