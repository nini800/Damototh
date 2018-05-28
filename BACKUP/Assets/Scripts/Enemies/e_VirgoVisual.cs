using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_VirgoVisual : e_Visual 
{

    new e_VirgoAI IA;
    bool lastFrameAttacking = false;
    int attackId = 0;
    protected override void Awake()
    {
        base.Awake();
        IA = (e_VirgoAI)base.IA;
    }

    protected override void Update ()
    {
        base.Update();
        bool thisFrameAttacking = (IA.CurAttackState & (e_EnemyAI.AttackState.Attacking | e_EnemyAI.AttackState.Casting | e_EnemyAI.AttackState.Recover)) != 0;
        if (!lastFrameAttacking && thisFrameAttacking)
            attackId = Random.Range(1, 3);
        else
            attackId = 0;
        switch (IA.CurVirgoState)
        {
            case e_VirgoAI.VirgoState.Waiting:
                anim.SetBool("VirgoAttacking", false);
                break;
            case e_VirgoAI.VirgoState.Attacking:
                anim.SetBool("VirgoAttacking", true);
                break;
            case e_VirgoAI.VirgoState.AttackSucceed:
                anim.SetBool("VirgoAttacking", false);
                break;
            case e_VirgoAI.VirgoState.Discovered:
                anim.SetBool("Waiting", false);
                anim.SetBool("VirgoAttacking", false);
                anim.SetFloat("MoveSpeed", Agent.velocity.SetY(0).magnitude);
                anim.SetInteger("Attacking", attackId);
                break;
        }



        lastFrameAttacking = thisFrameAttacking;
    }
	
}