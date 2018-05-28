using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_EnemyAI : e_Base 
{
    public enum HostilityType
    {
        Hostile,
        Neutral,
        Innocent
    }
    public enum AttackState
    {
        Normal,
        Casting,
        Attacking,
        Recover
    }
    public enum WeaponTypeEnum
    {
        Flesh,
        Metal
    }
    public enum BehaviourType
    {
        Mobile,
        Immobile
    }
    [Header("Movements")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float autoBrake;
    [SerializeField] protected float targetDistance;
    [SerializeField] protected float targetDistanceLaxism;
    [Header("Attacks")]
    [SerializeField] protected float minAttackWait;
    [SerializeField] protected float maxAttackWait;
    [SerializeField] protected EnemyAttackStats[] attacks;
    [Header("Others")]
    [SerializeField] protected HostilityType hostility;
    [SerializeField] protected AttackState curAttackState;
    [SerializeField] protected WeaponTypeEnum weaponType;
    [SerializeField] protected BehaviourType behaviour;

    new public HostilityType Hostility { get { return hostility; } }
    public AttackState CurAttackState { get { return curAttackState; } }
    public WeaponTypeEnum WeaponType { get { return weaponType; } }
    protected bool grounded = false;

    public float DistFromTarget { get { return (Body.position - targetBody.position.SetY(Body.position.y)).magnitude; } }
    public Vector3 ForwardTarget { get { return (targetBody.position.SetY(Body.position.y) - Body.position).normalized; } }
    protected Transform targetBody;
    public Transform Target { get { return targetBody; } }

    protected float lastAttackTime;

	protected virtual void Update ()
    {
        grounded = Physics.OverlapSphere(FeetColl.transform.position + FeetColl.center, FeetColl.radius, Game_NG.GroundLayerMask).Length > 0;
    }

    private void FixedUpdate()
    {
        Behaviour();
    }

    protected virtual void Behaviour()
    {
        if (grounded)
        {
            if (targetBody == null || behaviour == BehaviourType.Immobile)
            {
                Agent.velocity = Utilities.SubtractMagnitude(Agent.velocity, autoBrake * Utilities.Delta);

                if (behaviour != BehaviourType.Immobile)
                {
                    CheckAround(targetDistance);
                    SetDestination(Body.position);
                }

                return;
            }

            switch (curAttackState)
            {
                case AttackState.Normal:
                    //Move part
                    if (DistFromTarget > targetDistance + targetDistanceLaxism * 0.5f)
                        SetDestination(targetBody.position - ForwardTarget * targetDistance);
                    else if (DistFromTarget < targetDistance - targetDistanceLaxism * 0.5f)
                        SetDestination(targetBody.position - ForwardTarget * targetDistance);
                    else
                    {
                        Agent.SetDestination(Body.position);
                        StartCoroutine("AttackCoroutine");
                    }

                    //Update coll
                    BodyColl.transform.position = Body.position + Visual.forward * (BodyColl.transform.localPosition.SetY(0).magnitude) + Vector3.up * BodyColl.height * 0.5f;
                    break;
                case AttackState.Casting:
                    Agent.SetDestination(Body.position);
                    break;
                case AttackState.Attacking:
                    Agent.SetDestination(Body.position);
                    break;
            }
        }
    }

    public virtual void InterruptAttack()
    {
        StopCoroutine("AttackCoroutine");
        StopCoroutine("ImpulsesCoroutine");
        curAttackState = AttackState.Normal;
    }

    protected IEnumerator AttackCoroutine()
    {
        EnemyAttackStats attack = GetRandomAttack();
        if (attack != null)
        {
            StartCoroutine("ImpulsesCoroutine", attack.impulses);

            curAttackState = AttackState.Casting;
            yield return new WaitForSeconds(Random.Range(minAttackWait, maxAttackWait) + attack.castTime);

            curAttackState = AttackState.Attacking;
            try
            {
                GameObject o = Instantiate(attack.attackModel, Body.position, Visual.rotation, transform);
                AttackObject ao = o.GetComponent<AttackObject>();
                ao.Initialize(new HitInfos(EB, attack));
            }
            catch { Debug.LogError("No Prefab Found !"); }

            attack.lastAttackTime = Time.time;

            yield return new WaitForSeconds(attack.attackTime);

            curAttackState = AttackState.Recover;
            yield return new WaitForSeconds(attack.recoverTime);

            curAttackState = AttackState.Normal;
        }
    }
    protected IEnumerator ImpulsesCoroutine(AttackStats.Impulse[] impulses)
    {
        float count = 0f;

        for (int i = 0; i < impulses.Length; i++)
        {
            yield return new WaitForSeconds(impulses[i].time);

            count = 0f;
            while (count < impulses[i].duration)
            {
                Agent.velocity = Visual.TransformVector(impulses[i].impulse) * Utilities.Delta;
                yield return new WaitForFixedUpdate();
                count += Time.fixedDeltaTime;
            }
        }
    }

    protected EnemyAttackStats GetRandomAttack()
    {
        List<EnemyAttackStats> possibleAttacks = new List<EnemyAttackStats>();
        float totalFrequency = 0;
        for (int i = 0; i < attacks.Length; i++)
        {
            if (attacks[i].minHealthPercent <= LB.CurHealth/LB.MaxHealth*100 &&
                attacks[i].maxHealthPercent >= LB.CurHealth/LB.MaxHealth*100 &&
                attacks[i].lastAttackTime + attacks[i].attackCooldown < Time.time)
            {
                totalFrequency += attacks[i].frequency;
                possibleAttacks.Add(attacks[i]);
            }
        }

        if (possibleAttacks.Count == 0)
            return null;

        float rand = Random.Range(0f, totalFrequency);
        float cumulated = 0f;
        for (int i = 0; i < possibleAttacks.Count; i++)
        {
            cumulated += possibleAttacks[i].frequency;
            if (rand <= cumulated)
                return possibleAttacks[i];
        }

        Debug.LogError("Problem in frequency system");
        return null;
    }

    protected void CheckAround(float radius)
    {
        Collider[] colls;
        if ((colls = Physics.OverlapSphere(Body.position, radius, Game_NG.BeingLayerMask)).Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponentInParent<LivingBeing>().Faction_ != EB.Faction_)
                {
                    Debug.Log("b" + colls[i].name);
                    SetTarget(colls[0].GetComponentInParent<p_Base>().Body);
                    return;
                }
            }

            return;
        }
    }

    public void SetTarget(Transform target)
    {
        targetBody = target;
    }

    public void SetDestination(Vector3 pos)
    {
        if (pos != Agent.destination)
            Agent.SetDestination(pos);
    }

}