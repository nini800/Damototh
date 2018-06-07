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

    [System.Serializable]
    public class RandomFloat
    {
        public float min;
        public float max;

        public float choosenValue { get; private set; }

        public float Value { get { return baked ? choosenValue : Random.Range(min, max); }  }
        public void Bake()
        {
            choosenValue = Value;
            baked = true;
        }
        private bool baked = false;
    }

    [Header("Movements")]
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float rotationSpeed;
    [SerializeField] protected float autoBrake;
    [SerializeField] protected float targetDetectDistance = 5;
    [SerializeField] protected RandomFloat maxTargetDistance;
    [Space]
    [SerializeField] protected RandomFloat dodgeWaitTime;
    [SerializeField] protected float dodgeChances;
    [SerializeField] protected RandomFloat dodgeDetectRadius;
    [SerializeField] protected RandomFloat dodgeSpeed;
    [SerializeField] protected RandomFloat dodgeRandomAngle;
    [SerializeField] protected RandomFloat dodgeCooldown;
    [Header("Attacks")]
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
    protected EnemyAttackStats currentAttack;

    public float DistFromTarget { get { return (Body.position - targetBody.position.SetY(Body.position.y)).magnitude; } }
    public Vector3 ForwardTarget { get { return (targetBody.position.SetY(Body.position.y) - Body.position).normalized; } }
    protected Transform targetBody;
    public Transform Target { get { return targetBody; } }

    protected float lastAttackTime, lastDodgeTime = -Mathf.Infinity;

    protected override void Awake()
    {
        base.Awake();

        Agent.speed = moveSpeed;
        Agent.updateRotation = false;
        maxTargetDistance.Bake();
        dodgeCooldown.Bake();
    }

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
                    SetDestination(Body.position);
                    CheckAround(targetDetectDistance);
                }

                return;
            }


            switch (curAttackState)
            {
                case AttackState.Normal:
                    CanAttackHandler();
                    break;
                case AttackState.Casting:
                    CastingAttackHandler();
                    break;
                case AttackState.Attacking:
                    AttackingHandler();
                    break;
            }

            ListenTargetAttacks();

            //Update coll
            BodyColl.transform.position = Body.position + Visual.forward * (BodyColl.transform.localPosition.SetY(0).magnitude) + Vector3.up * BodyColl.height * 0.5f;
        }
    }

    public virtual void CanAttackHandler()
    {
        if (DistFromTarget > maxTargetDistance.Value)
        {
            SetDestination(targetBody.position);
            if (Agent.desiredVelocity.sqrMagnitude != 0)
                Body.rotation = Quaternion.RotateTowards(Body.rotation, Quaternion.LookRotation(Agent.desiredVelocity), rotationSpeed);
        }
        else
        {
            Agent.SetDestination(Body.position);
            //print("A: " + ((targetBody.position - Body.position).SetY(0).normalized) + " / "  + targetBody.position + " / " + Body.position);
            Body.rotation = Quaternion.RotateTowards(Body.rotation, Quaternion.LookRotation((targetBody.position - Body.position).SetY(0).normalized), rotationSpeed);
            StartCoroutine("AttackCoroutine");
        }
    }

    AttackState e_oldAttackState = AttackState.Normal;
    p_AttackController.e_AttackState p_oldAttackState = p_AttackController.e_AttackState.None;
    public virtual void ListenTargetAttacks()
    {
        e_EnemyAI enemy;
        p_Base player;
        if (enemy = targetBody.GetComponentInParent<e_EnemyAI>())
        {
            if ((e_oldAttackState != AttackState.Casting) && enemy.CurAttackState == AttackState.Casting)
            {
                SendMessage("OnTargetBeginCastingAttack", enemy.currentAttack);
            }
            else if (e_oldAttackState == AttackState.Casting && enemy.CurAttackState == AttackState.Attacking)
            {
                SendMessage("OnTargetBeginAttacking", enemy.currentAttack);
            }

            e_oldAttackState = enemy.CurAttackState;
        }
        else if (player = targetBody.GetComponentInParent<p_Base>())
        {
            if (p_oldAttackState != p_AttackController.e_AttackState.Casting && player.AttackState == p_AttackController.e_AttackState.Casting)
            {
                SendMessage("OnTargetBeginCastingAttack", player.AC.CurrentAttack);
            }
            else if (p_oldAttackState == p_AttackController.e_AttackState.Casting && player.AttackState == p_AttackController.e_AttackState.Attacking)
            {
                SendMessage("OnTargetBeginAttacking", player.AC.CurrentAttack);
            }

            p_oldAttackState = player.AttackState;
        }
    }

    public virtual void CastingAttackHandler()
    {
        Agent.velocity = Utilities.SubtractMagnitude(Agent.velocity, autoBrake * Utilities.Delta);
        Agent.SetDestination(Body.position);
    }
    public virtual void AttackingHandler()
    {
        Agent.velocity = Utilities.SubtractMagnitude(Agent.velocity, autoBrake * Utilities.Delta);
        Agent.SetDestination(Body.position);
    }
    public virtual void InterruptAttack()
    {
        StopCoroutine("AttackCoroutine");
        StopCoroutine("ImpulsesCoroutine");
        curAttackState = AttackState.Normal;
    }

    protected IEnumerator AttackCoroutine()
    {
        currentAttack = GetRandomAttack();
        if (currentAttack != null)
        {
            SendMessage("OnAttackStart", currentAttack);
            StartCoroutine("ImpulsesCoroutine", currentAttack.impulses);

            curAttackState = AttackState.Casting;
            yield return new WaitForSeconds(currentAttack.castTime);

            curAttackState = AttackState.Attacking;
            try
            {
                GameObject o = Instantiate(currentAttack.attackModel, Body.position, Visual.rotation, transform);
                AttackObject ao = o.GetComponent<AttackObject>();
                ao.Initialize(new HitInfos(EB, currentAttack));
            }
            catch { Debug.LogError("No Prefab Found !"); }

            currentAttack.lastAttackTime = Time.time;

            yield return new WaitForSeconds(currentAttack.attackTime);

            curAttackState = AttackState.Recover;
            yield return new WaitForSeconds(currentAttack.recoverTime);

            SendMessage("OnAttackStop", currentAttack);
            currentAttack = null;
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
            //print(Vector2.Angle(Body.forward.ToXZ(), (targetBody.position - Body.position).normalized.ToXZ()));
            if (attacks[i].minHealthPercent <= LB.CurHealth/LB.MaxHealth*100 &&
                attacks[i].maxHealthPercent >= LB.CurHealth/LB.MaxHealth*100 &&
                attacks[i].lastAttackTime + attacks[i].attackCooldown < Time.time &&
                attacks[i].minDistance >= Vector3.Distance(targetBody.position, Body.position) &&
                attacks[i].minAngle >= Vector2.Angle(Body.forward.ToXZ(), (targetBody.position - Body.position).ToXZ().normalized))
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

    protected void CheckAround(float maxRadius)
    {
        Collider[] colls;
        if ((colls = Physics.OverlapSphere(Body.position, maxRadius, Game_NG.BeingLayerMask)).Length > 0)
        {
            for (int i = 0; i < colls.Length; i++)
            {
                if (colls[i].GetComponentInParent<LivingBeing>().Faction_ != EB.Faction_)
                {
                    SetTarget(colls[0].GetComponentInParent<Thing_Base>().Body);
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


    void OnTargetBeginCastingAttack(AttackStats attack)
    {
        if (Time.time > lastDodgeTime + dodgeCooldown.Value && DistFromTarget <= dodgeDetectRadius.Value && Random.Range(0, 100f) > dodgeChances)
            Invoke("Dodge", dodgeWaitTime.Value);
    }
    void OnTargetBeginAttacking(AttackStats attack)
    {
    }
    void Dodge()
    {
        if (EB.CurLivingState != LivingBeing.LivingState.Normal)
            return;

        lastDodgeTime = Time.time;

        Vector3 dir = (Body.position - targetBody.position).SetY(0).normalized;
        //Debug.DrawRay(Body.position, dir, Color.red, 100f);
        Transform temp = new GameObject().transform;
        temp.LookAt(dir);
        dir = temp.InverseTransformDirection(dir);
        dir.x += dodgeRandomAngle.Value;
        dir = temp.TransformDirection(dir);
        dir.Normalize();
        //Debug.DrawRay(Body.position, dir, Color.yellow, 100f);

        dodgeCooldown.Bake();
        Agent.velocity += dir * dodgeSpeed.Value;
    }

    private void OnValidate()
    {
        if (dodgeChances < 0)
            dodgeChances = 0;
        else if (dodgeChances > 100)
            dodgeChances = 100;
    }

}