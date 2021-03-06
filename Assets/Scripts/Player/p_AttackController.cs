﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class p_AttackController : p_Base 
{
    public enum e_AttackState
    {
        None = 1,
        Casting = 2,
        Attacking = 4,
        Recovering = 8,
        Mutilating = 16,
        MutilateRecover = 32
    }

    //Getters
    public LayerMask whatIsEnemy { get { return LayerMask.GetMask("Enemy"); } }

    //Members
    //Public
    public new e_AttackState AttackState { get { return attackState; } }
    public PlayerAttackStats CurrentAttack { get { return currentAttack; } }

    public e_AttackState attackState;

    public PlayerAttackStats[] baseAttacks;
    public bool attackFold = false;

    new public float LockRange { get { return lockRange; } }
    public bool IsLocked { get; protected set; }

    new public e_EnemyAI EnemyLocked { get; set; }
    //Private
    PlayerAttackStats currentAttack = null;

    public float lockRange;

    float? mutilateStartTime = null;

    public float mutilateCastTime = 1f;
    public float mutilateRecoverTime = 1f;
    public float mutilateHealthCost = 25f;
    public float mutilateMinBloodGain = 5f;
    public float mutilateBloodGain = 10f;
    public float mutilateMadnessGain = 55f;

    public float strongBloodThreshold = 50;

    protected void Update ()
    {
        AttackHandler();
	}

    void AttackHandler()
    {
        if ((PB.CurLivingState & (LivingBeing.LivingState.Stunned | LivingBeing.LivingState.Dead)) == 0 && InteractionState != p_InteractionController.e_InteractionState.Interacting)
        {
            if (currentAttack == null)
                CheckAttacks(baseAttacks);
            else
                CheckAttacks(currentAttack.ComboAttacks);

            MutilateHandler();
        }

        LockHandler();
    }

    void CheckAttacks(PlayerAttackStats[] attacks)
    {
        if ((attackState & (e_AttackState.Recovering | e_AttackState.None)) == 0)
            return;

        for (int i = 0; i < attacks.Length; i++)
        {
            if ((attacks[i].MoveInput & MovementState) != 0)
            {
                if ((C.LightAttack && attacks[i].AttackInput == PlayerAttackStats.AttackInputType.Light) ||
                    (C.HeavyAttack && attacks[i].AttackInput == PlayerAttackStats.AttackInputType.Heavy))
                {
                    SendMessage("OnCastAttack", attacks[i]);

                    StopCoroutine("AttackCoroutine");
                    StopCoroutine("ImpulsesCoroutine");
                    StartCoroutine("AttackCoroutine", attacks[i]);
                    StartCoroutine("ImpulsesCoroutine", attacks[i].impulses);
                }
            }
        }
    }
    IEnumerator AttackCoroutine(PlayerAttackStats attack)
    {
        currentAttack = attack;
        attackState = e_AttackState.Casting;

        yield return new WaitForSeconds(attack.CastTime);

        attackState = e_AttackState.Attacking;

        try
        {
            GameObject o = Instantiate(attack.attackModel, Body.position, Visual.rotation, transform);
            AttackObject ao = o.GetComponent<AttackObject>();
            ao.Initialize(new HitInfos(PB, attack), GetWeightByThreshold(attack.weight));
        }
        catch { Debug.LogError("No Prefab Found !"); }

        SendMessage("OnAttackStart", attack);
        yield return new WaitForSeconds(attack.AttackTime);
        SendMessage("OnAttackStop", attack);

        attackState = e_AttackState.Recovering;
        yield return new WaitForSeconds(attack.RecoverTime);

        attackState = e_AttackState.None;
        currentAttack = null;
    }
    IEnumerator ImpulsesCoroutine(AttackStats.Impulse[] impulses)
    {
        float count = 0f;

        for (int i = 0; i < impulses.Length; i++)
        {
            yield return new WaitForSeconds(impulses[i].time);

            count = 0f;
            while(count < impulses[i].duration)
            {
                R.velocity = Visual.TransformVector(impulses[i].impulse);
                yield return new WaitForFixedUpdate();
                count += Time.fixedDeltaTime;
            }
        }
    }

    AttackStats.AttackWeightType GetWeightByThreshold(AttackStats.AttackWeightType inWeight)
    {
        if (PB.CurBlood < strongBloodThreshold)
        {
            return inWeight;
        }
        else
        {
            switch (inWeight)
            {
                case AttackStats.AttackWeightType.Cheap:
                    return AttackStats.AttackWeightType.Light;
                case AttackStats.AttackWeightType.Light:
                    return AttackStats.AttackWeightType.Medium;
                default:
                    return AttackStats.AttackWeightType.Heavy;
            }
        }
    }

    public void InterruptAttack()
    {
        StopCoroutine("AttackCoroutine");
        StopCoroutine("ImpulsesCoroutine");

        attackState = e_AttackState.None;
        currentAttack = null;
    }

    void MutilateHandler()
    {
        if (currentAttack != null)
            return;

        if (C.MutilateDown)
        {
            //Start Mutilate
            mutilateStartTime = Time.time;
            attackState = e_AttackState.Mutilating;
        }
        else if (mutilateStartTime != null)
        {
            if (C.MutilateHeld)
            {
                if (mutilateStartTime + mutilateCastTime > Time.time)
                {
                    //Mutilate progress
                    PostProcess.SetMutilatePP((Time.time - (float)mutilateStartTime) / mutilateCastTime);
                }
                else if(mutilateStartTime + mutilateCastTime + mutilateRecoverTime > Time.time)
                {
                    //Reset mutilate
                    if (attackState != e_AttackState.MutilateRecover)
                    {
                        if (PB.CurHealth > mutilateHealthCost)
                        {
                            PB.AddHealth(-mutilateHealthCost);
                            PB.AddBlood(mutilateMinBloodGain + mutilateBloodGain);
                        }
                        else
                        {
                            if (PB.CurHealth > 1)
                            {
                                PB.AddBlood(mutilateBloodGain * ((PB.CurHealth - 1) / mutilateHealthCost));
                                PB.AddHealth(-PB.CurHealth + 1);
                            }

                            PB.AddBlood(mutilateMinBloodGain);
                        }


                        PB.AddMadness(mutilateMadnessGain);

                        PostProcess.SetMutilatePP(0);
                        FX.SpawnHitFX(BodyRaycastOrigin.position + Visual.forward * BodyColl.radius, Visual.forward, transform);
                        attackState = e_AttackState.MutilateRecover;
                    }
                }
                else
                {
                    PostProcess.SetMutilatePP(0);
                    attackState = e_AttackState.None;
                    mutilateStartTime = null;
                }
            }
            else if (attackState != e_AttackState.MutilateRecover || (float)mutilateStartTime + mutilateCastTime + mutilateRecoverTime <= Time.time)
            {
                PostProcess.SetMutilatePP(0);
                attackState = e_AttackState.None;
                mutilateStartTime = null;
            }
        }
    }

    void LockHandler()
    {
        if (C.Lock)
        {
            if (IsLocked)
            {
                Unlock();
            }
            else
            {
                e_EnemyAI[] enemies = CC.GetEnemiesOnScreen(lockRange, whatIsEnemy);
                if (enemies.Length <= 0)
                    return;

                float distFromCenter = Mathf.Infinity;
                float temp = 0f;

                for (int i = 0; i < enemies.Length; i++)
                {
                    if ((temp = Mathf.Abs(Cam.WorldToScreenPoint(enemies[i].transform.position).x - Screen.width*0.5f)) < distFromCenter)
                    {
                        distFromCenter = temp;
                        Lock(enemies[i].GetComponentInParent<e_EnemyAI>());
                    }
                }
            }
        }
        else if (IsLocked && EnemyLocked.IsDead)
        {
            Unlock();
        }
    }
    void Lock(e_EnemyAI enemy)
    {
        EnemyLocked = enemy;
        IsLocked = true;
        PostProcess.SetLockPP(true);
    }
    public void Unlock()
    {
        EnemyLocked = null;
        IsLocked = false;
        PostProcess.SetLockPP(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.Find("Body").position, lockRange);
    }
}