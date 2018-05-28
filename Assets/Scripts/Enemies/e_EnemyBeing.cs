using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_EnemyBeing : LivingBeing 
{
    [Header("Blood and Madness params")]
    [SerializeField] float bloodFactor;
    [SerializeField] float bloodOnDie;
    [SerializeField] float madnessFactor;
    [SerializeField] float madnessOnDie;

    protected new e_Base linkedThing;

    protected override void Awake()
    {
        base.Awake();
        linkedThing = (e_Base)base.linkedThing;
    }

    public void TakeHit(HitInfos hitInfos, out float blood, out float madness)
    {
        if (CurLivingState == LivingState.Dead)
        {
            blood = 0f;
            madness = 0;
            return;
        }

        if (linkedThing.IA.CurAttackState == e_EnemyAI.AttackState.Attacking && linkedThing.IA.WeaponType == e_EnemyAI.WeaponTypeEnum.Metal)
        {
            linkedThing.IA.InterruptAttack();

            if (linkedThing.IA.WeaponType == e_EnemyAI.WeaponTypeEnum.Metal && linkedThing.Visual.InverseTransformDirection(hitInfos.Direction).z > 0)
            {
                blood = 0f;
                madness = 0f;
                Debug.Log("MonsterParry");
                return;
            }
        }

        float diffHealth = curHealth;

        base.TakeHit(hitInfos);

        linkedThing.Agent.velocity = hitInfos.AttackObject.transform.TransformVector(hitInfos.AttackStats.knockback);

        diffHealth -= curHealth;//here diffhealth = difference of life before and after hit

        blood = bloodFactor * diffHealth;
        float factor = 0;
        switch (linkedThing.Hostility)
        {
            case e_EnemyAI.HostilityType.Hostile:
                factor *= 1;
                break;
            case e_EnemyAI.HostilityType.Neutral:
                factor *= 2;
                break;
            case e_EnemyAI.HostilityType.Innocent:
                factor *= 3;
                break;
        }
        madness = madnessFactor * diffHealth * factor;

        if (CurLivingState == LivingState.Dead)
        {
            madness += madnessOnDie;
            blood += bloodOnDie;
        }
    }
}