using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitInfos
{
    public LivingBeing Attacker { get; protected set; }
    public LivingBeing Attacked { get; protected set; }

    public AttackStats AttackStats { get; protected set; }
    public AttackObject AttackObject { get; protected set; }

    public EnemyAttackStats EnemyAttackStats { get; protected set; }
    public PlayerAttackStats PlayerAttackStats { get; protected set; }

    public Vector3 HitPoint { get; protected set; }
    public Vector3 Direction { get; protected set; }

    public float BloodGot { get; protected set; }
    public float MadnessGot { get; protected set; }


    public HitInfos(LivingBeing Attacker, AttackStats AttackStats)
    {
        this.Attacker = Attacker;
        this.AttackStats = AttackStats;
    }

    public HitInfos(LivingBeing Attacker, PlayerAttackStats AttackStats) : this(Attacker, (AttackStats)AttackStats)
    {
        PlayerAttackStats = AttackStats;
    }
    public HitInfos(LivingBeing Attacker, EnemyAttackStats AttackStats) : this(Attacker, (AttackStats)AttackStats)
    {
        EnemyAttackStats = AttackStats;
    }

    public HitInfos(HitInfos knownInfos)
    {
        if (knownInfos.Attacker != null)
            Attacker = knownInfos.Attacker;
        if (knownInfos.Attacked != null)
            Attacked = knownInfos.Attacked;

        if (knownInfos.AttackStats != null)
            AttackStats = knownInfos.AttackStats;
        if (knownInfos.AttackObject != null)
            AttackObject = knownInfos.AttackObject;

        if (knownInfos.EnemyAttackStats != null)
            EnemyAttackStats = knownInfos.EnemyAttackStats;
        if (knownInfos.PlayerAttackStats != null)
            PlayerAttackStats = knownInfos.PlayerAttackStats;

        if (knownInfos.HitPoint != Vector3.zero)
            HitPoint = knownInfos.HitPoint;
        if (knownInfos.Direction != Vector3.zero)
            Direction = knownInfos.Direction;

        if (knownInfos.BloodGot != 0)
            BloodGot = knownInfos.BloodGot;
        if (knownInfos.MadnessGot != 0)
            MadnessGot = knownInfos.MadnessGot;
    }

    public HitInfos(HitInfos knownInfos, AttackObject AttackObject) : this(knownInfos)
    {
        this.AttackObject = AttackObject;
    }

    public HitInfos(HitInfos knownInfos, LivingBeing Attacked) : this (knownInfos)
    {
        this.Attacked = Attacked;
    }

    public HitInfos(HitInfos knownInfos, Vector3 PointOrDirValue, bool PointOrDir) : this (knownInfos)
    {
        if (PointOrDir)
            this.HitPoint = PointOrDirValue;
        else
            this.Direction = PointOrDirValue;
    }

    public HitInfos(HitInfos knownInfos, Vector3 HitPoint, Vector3 Direction) : this(knownInfos)
    {
            this.HitPoint = HitPoint;
            this.Direction = Direction;
    }

    public HitInfos(HitInfos knownInfos, float bloodGot, float madnessGot) : this(knownInfos)
    {
        this.BloodGot = bloodGot;
        this.MadnessGot = madnessGot;
    }

}
