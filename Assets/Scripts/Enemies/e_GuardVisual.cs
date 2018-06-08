using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_GuardVisual : e_Visual
{
    protected override void Update()
    {
        base.Update();

        if (IA.CurAttackState == e_EnemyAI.AttackState.Casting)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.yellow;
        }
        else if (IA.CurAttackState == e_EnemyAI.AttackState.Attacking)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.red;
        }
        else if (IA.CurAttackState == e_EnemyAI.AttackState.Recover)
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.cyan;
        }
        else
        {
            GetComponentInChildren<MeshRenderer>().material.color = Color.white;
        }
    }
}
