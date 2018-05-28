using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_PlayerBeing : LivingBeing 
{
	[Header("Blood and Madness")]
    [SerializeField] protected float maxBlood;
    [SerializeField] protected float startBlood;
    [SerializeField] protected float maxMadness, startMadness, madnessLossPerSecond = 2f;
    [SerializeField][Range(0f, 1f)] protected float euphoriaThreshold;

    public float MaxBlood { get { return maxBlood; } }
    public float CurBlood { get { return curBlood; } }

    public float MaxMadness { get { return maxMadness; } }
    public float CurMadness { get { return curMadness; } }
    public float EuphoriaThreshold { get { return euphoriaThreshold; } }

    public bool IsFullBlood { get { return curBlood >= maxBlood; } }

    protected float curBlood, curMadness;

    new protected p_Base linkedThing;

    protected override void Awake()
    {
        base.Awake();

        curBlood = startBlood;
        curMadness = startMadness;

        linkedThing = (p_Base)base.linkedThing;
    }

    protected virtual void Update()
    {
        AddMadness(-madnessLossPerSecond * Time.deltaTime);
        PostProcess.SetMadnessPP(curMadness / maxMadness);
    }

    public override void Die(bool destroy = false)
    {
        base.Die(destroy);

        CurLivingState = LivingState.Normal;
        linkedThing.R.isKinematic = false;
        AddHealth(1000);
        AddMadness(-1000);
        AddBlood(-1000);
        linkedThing.AC.Unlock();
        linkedThing.R.velocity = Vector3.zero;
    }

    public void AddBlood(float blood)
    {
        curBlood = Utilities.AddWithLimit(curBlood, blood, 0, maxBlood);
    }
    public void AddMadness(float madness)
    {
        curMadness = Utilities.AddWithLimit(curMadness, madness, 0, maxMadness);
    }

    void OnAttackHit(HitInfos infos)
    {
        AddBlood(infos.BloodGot);
        AddMadness(infos.MadnessGot);
    }

    public override void TakeHit(HitInfos hit)
    {
        if (CurLivingState == LivingState.Dead)
            return;

        if (linkedThing.AttackState == p_AttackController.e_AttackState.Attacking)
        {
            linkedThing.AC.InterruptAttack();

            if(linkedThing.Visual.InverseTransformDirection(hit.Direction).z > 0)
            {
                Debug.Log("PlayerParry");
                return;
            }
        }

        if (linkedThing != null)
        {
            Vector3 knockback = Utilities.SubtractMagnitude(hit.AttackStats.knockback, knockbackResistance);
            linkedThing.R.AddForce(hit.Direction.SetY(0).normalized * knockback.SetY(0).magnitude + Vector3.zero.SetY(knockback.y), ForceMode.VelocityChange);
        }

        hit.AttackStats.stunForce = Utilities.SubtractFloat(hit.AttackStats.stunForce, stunResistance);
        if (hit.AttackStats.stunForce >= maxStunValue)
        {
            StopCoroutine("StunCoroutine");
            StartCoroutine("StunCoroutine", hit.AttackStats.stunTime);
        }

        AddHealth(-hit.AttackStats.damages);
        if (curHealth == 0)
            Die();
    }

    public override IEnumerator StunCoroutine(float stunTime)
    {
        CurLivingState = LivingState.Stunned;
        linkedThing.R.isKinematic = true;
        float count = 0f;
        while (count < stunTime)
        {
            yield return null;
            count += Time.deltaTime;
        }
        CurLivingState = LivingState.Normal;
        linkedThing.R.isKinematic = false;
    }
    
    protected override void OnValidate()
    {
        base.OnValidate();

        if (maxBlood < 0)
            maxBlood = 0;
        if (maxMadness < 0)
            maxMadness = 0;

        if (startBlood > maxBlood)
            startBlood = maxBlood;

        if (startMadness > maxMadness)
            startMadness = maxMadness;
    }
}