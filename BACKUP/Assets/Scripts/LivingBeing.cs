using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBeing : MonoBehaviour 
{
    public enum LivingState
    {
        Normal,
        Stunned,
        Dead
    }

    public enum Faction
    {
        Player,
        Enemy,
        Guard
    }
    [SerializeField] LivingState livingState;
    [SerializeField] Faction faction;
    [Header("Life and Knockback")]
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float startHealth;
    [SerializeField] protected float knockbackResistance;

    [Header("Stun")]
    [SerializeField] protected float maxStunValue;
    [SerializeField] protected float stunResistance;

    public LivingState CurLivingState { get { return livingState; } protected set { livingState = value; } }
    public Faction Faction_ { get { return faction; } protected set { faction = value; } }
    public float MaxHealth { get { return maxHealth; } }
    public float CurHealth { get { return curHealth; } }

    public bool IsFullHealth { get { return curHealth >= maxHealth; } }

    [System.NonSerialized] protected Thing_Base linkedThing;
    public Thing_Base LinkedThing { get { return linkedThing; } }
    public Transform Body { get { return linkedThing.Body; } }

    protected float curHealth;
    protected virtual void Awake()
    {
        linkedThing = GetComponent<Thing_Base>();
        curHealth = startHealth;
    }

    public virtual void AddHealth(float health)
    {
        curHealth = Utilities.AddWithLimit(curHealth, health, 0, maxHealth);
    }

    public virtual void TakeHit(HitInfos hitInfos)
    {
        if (CurLivingState == LivingState.Dead)
            return;

        linkedThing.SendMessage("OnTakeHit", hitInfos, SendMessageOptions.DontRequireReceiver);

        hitInfos.AttackStats.stunForce = Utilities.SubtractFloat(hitInfos.AttackStats.stunForce, stunResistance);
        if (hitInfos.AttackStats.stunForce >= maxStunValue)
        {
            StopCoroutine("StunCoroutine");
            StartCoroutine("StunCoroutine", hitInfos.AttackStats.stunTime);
        }

        AddHealth(-hitInfos.AttackStats.damages);
        if (curHealth == 0)
            Die();
    }

    public virtual void Die(bool destroy = true)
    {
        StopCoroutine("StunCoroutine");
        CurLivingState = LivingState.Dead;
        gameObject.SendMessage("OnDie", SendMessageOptions.DontRequireReceiver);
        if (destroy)
        Destroy(gameObject, 0f);
    }

    public virtual IEnumerator StunCoroutine(float stunTime)
    {
        CurLivingState = LivingState.Stunned;

        yield return new WaitForSeconds(stunTime);
        CurLivingState = LivingState.Normal;
    }

    protected virtual void OnValidate()
    {
        if (maxHealth < 0)
            maxHealth = 0;

        if (startHealth > maxHealth)
            startHealth = maxHealth;
    }
}