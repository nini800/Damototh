using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_Base : Thing_Base 
{
    public CapsuleCollider BodyColl { get; protected set; }
    public SphereCollider HeadColl { get; protected set; }
    public Transform BodyVisual { get; protected set; }
    public Transform HeadVisual { get; protected set; }
    public Transform FeetVisual { get; protected set; }
    public Transform CamPivot { get; protected set; }
    public Transform CamSecondPivot { get; protected set; }
    public Transform CamArm { get; protected set; }
    public Transform AnimPivot { get; protected set; }
    public Transform Cam_t { get; protected set; }
    public Transform LockArrow { get; protected set; }
    public Camera Cam { get; protected set; }

    public Rigidbody R { get; protected set; }

    public p_CameraController CC { get; protected set; }
    public p_MovementController MC { get; protected set; }
    public p_AttackController AC { get; protected set; }
    public p_InteractionController IC { get; protected set; }
    public p_Visual V { get; protected set; }
    public p_PlayerBeing PB { get; protected set; }

    public p_Controller C { get; protected set; }

    public e_EnemyAI EnemyLocked { get { return AC.EnemyLocked; } set { AC.EnemyLocked = value; } }

    public bool Grounded { get { return MC.Grounded; } }
    public bool Sprinting { get { return MC.Sprinting; } }
    public bool Dashing { get { return MC.Dashing; } }

    public float LockRange { get { return AC.LockRange; } }

    public p_MovementController.e_MovementState MovementState { get { return MC.MovementState; } }
    public p_AttackController.e_AttackState AttackState { get { return AC.AttackState; } }
    public p_InteractionController.e_InteractionState InteractionState { get { return IC.InteractionState; } }

    protected override void Awake ()
    {
        base.Awake();

        R = Body.GetComponent<Rigidbody>();
        BodyColl = Body.Find("BodyColl").GetComponent<CapsuleCollider>();

        CamPivot = transform.Find("CamPivot");
        CamSecondPivot = CamPivot.Find("CamSecondPivot");
        CamArm = CamSecondPivot.Find("CamArm");
        AnimPivot = CamArm.Find("AnimPivot");
        Cam_t = AnimPivot.Find("Camera");
        Cam = Cam_t.GetComponent<Camera>();


        LockArrow = transform.Find("LockArrowCanvas");

        CC = GetComponent<p_CameraController>();
        MC = GetComponent<p_MovementController>();
        AC = GetComponent<p_AttackController>();
        IC = GetComponent<p_InteractionController>();
        V = GetComponent<p_Visual>();
        PB = (p_PlayerBeing)LB;

        C = GetComponent<p_Controller>();
    }
	
}