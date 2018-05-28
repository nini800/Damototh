using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thing_Base : MonoBehaviour
{
    public Transform Body { get; protected set; }
    public Transform BodyRaycastOrigin { get; protected set; }
    public Animator anim { get; protected set; }

    public Transform Visual { get; protected set; }

    public bool IsDead { get { return LB.CurLivingState == LivingBeing.LivingState.Dead; } }

    public LivingBeing LB { get; protected set; }

    protected virtual void Awake()
    {
        Body = transform.Find("Body");
        BodyRaycastOrigin = Body.Find("BodyRaycastOrigin");

        Visual = transform.Find("Visual");
        anim = Visual.GetComponent<Animator>();
        LB = GetComponent<LivingBeing>();
    }
}