using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_GuardVisual : e_Visual
{
    protected override void AnimationHandler()
    {
        if (IA.CurAttackState != e_EnemyAI.AttackState.Normal)
        {
            PlayAnimation("guardattack_combo", 0.2f);
        }
        else if (Agent.desiredVelocity.magnitude > 0.1f)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }
}
