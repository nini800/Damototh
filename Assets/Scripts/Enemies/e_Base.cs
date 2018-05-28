using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class e_Base : Thing_Base 
{

    public CapsuleCollider BodyColl { get; protected set; }
    public SphereCollider FeetColl { get; protected set; }

    public NavMeshAgent Agent { get; protected set; }

    public e_EnemyAI IA { get; protected set; }
    public e_EnemyBeing EB { get; protected set; }

    public e_EnemyAI.HostilityType Hostility { get { return IA.Hostility; } }

    protected override void Awake()
    {
        base.Awake();

        Agent = Body.GetComponent<NavMeshAgent>();
        BodyColl = Body.Find("BodyColl").GetComponent<CapsuleCollider>();
        FeetColl = Body.Find("FeetColl").GetComponent<SphereCollider>();

        EB = (e_EnemyBeing)LB;
        IA = GetComponent<e_EnemyAI>();

    }

}