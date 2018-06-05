using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackStats
{
    public enum AttackWeightType
    {
        Cheap = 1,
        Light = 2,
        Medium = 4,
        Heavy = 8,
        Unstoppable = 16
    }
    [Header("Stats")]
    public string attackName;
    public string Name { get { return attackName; } set { attackName = value; } }

    public float damages;
    public Vector3 knockback;
    public AttackWeightType weight;
    public float stunForce;
    public float stunTime = 1;

    public bool followCaster;
    public float hitboxLifeTime;
    public int maxHitTimes = 1;


    [Header("Timings")]
    public float castTime;
    public float attackTime;
    public float recoverTime;

    public float CastTime { get { return castTime; } set { castTime = value; } }
    public float AttackTime { get { return attackTime; } set { attackTime = value; } }
    public float RecoverTime { get { return recoverTime; } set { recoverTime = value; } }

    [Header("Impulses")]
    public Impulse[] impulses;

    [Header("Misc")]
    public GameObject attackModel;

    [System.Serializable]
    public class Impulse
    {
        public Vector3 impulse;
        public float time;
        public float duration;

        [HideInInspector]
        public bool fold;
    }
}

[System.Serializable]
public class EnemyAttackStats : AttackStats
{
    [Header("Conditions")]
    public float frequency = 1;
    public float minHealthPercent = 0;
    public float maxHealthPercent = 100;
    public float minDistance = 3;
    public float minAngle = 5;
    public float attackCooldown;


    [System.NonSerialized] public float lastAttackTime = -Mathf.Infinity;
}

[System.Serializable]
public class PlayerAttackStats : AttackStats
{
    [SerializeField] AttackInputType attackInput;
    [SerializeField] p_MovementController.e_MovementState moveInput;

    public PlayerAttackStats[] ComboAttacks;

    public AttackInputType AttackInput { get { return attackInput; } set { attackInput = value; } }
    public p_MovementController.e_MovementState MoveInput { get { return moveInput; } set { moveInput = value; } }

    [HideInInspector]
    public bool fold = false, comboFold = false, impulseFold;

    public PlayerAttackStats()
    {
        ComboAttacks = new PlayerAttackStats[0];
        Name = "New Attack";
        attackInput = AttackInputType.Light;
        impulses = new Impulse[0];
    }

    public enum AttackInputType
    {
        Light = 1,
        Heavy = 2
    }
}