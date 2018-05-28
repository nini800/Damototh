using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_VirgoAI : e_EnemyAI 
{
    public enum VirgoState
    {
        Waiting,
        Attacking,
        AttackSucceed,
        Discovered
    }

    [Header("Virgo AI")]
    [SerializeField] VirgoState curVirgoState = VirgoState.Waiting;
    [SerializeField] float trapSpeed = 1f;
    [SerializeField] float trapMaxDist = 1f;
    [SerializeField] float virgoMass = 500;
    [SerializeField] float sideStepSpeed = 5f;
    [SerializeField] float nearDistBeforeAttacking = 3f;

    [SerializeField] AttackStats grabAttackStats;

    Transform linkedVirgo;
    Transform posInVirgo;
    AttackObject grabAttack;
    Animator virgoAnimator;

    Vector3 oPos;
    
    public VirgoState CurVirgoState { get { return curVirgoState; } }

    protected override void Awake()
    {
        base.Awake();

        oPos = Body.position;

        linkedVirgo = Body.Find("Virgo");
        if (linkedVirgo != null)
        {
            posInVirgo = linkedVirgo.Find("PosInVirgo");

            virgoAnimator = linkedVirgo.GetComponent<Animator>();

            grabAttack = linkedVirgo.Find("Grab").GetComponent<AttackObject>();
            grabAttack.Initialize(new HitInfos(EB, grabAttackStats));
        }
    }

    protected override void Update ()
    {
        VirgoBehaviour();
    }

    protected virtual void VirgoBehaviour()
    {
        switch (CurVirgoState)
        {
            case VirgoState.Waiting:
                CheckAround(nearDistBeforeAttacking);

                if (targetBody != null)
                    GiveUpVirgo();


                SetDestination(oPos);
                break;
            case VirgoState.Attacking:

                SetDestination(oPos + Body.forward * trapMaxDist);

                if ((Body.position - oPos).magnitude >= trapMaxDist)
                    curVirgoState = VirgoState.Waiting;
                break;
            case VirgoState.AttackSucceed:
                SetDestination((targetBody.position + (Body.position - posInVirgo.position)).SetY(Body.position.y));

                if (targetBody.GetComponentInParent<LivingBeing>().CurLivingState != LivingBeing.LivingState.Stunned)
                    GiveUpVirgo();
                break;
            case VirgoState.Discovered:
                base.Update();
                break;
        }
    }

    void OnTakeHit(HitInfos hit)
    {
        if (curVirgoState != VirgoState.Discovered)
        {
            GiveUpVirgo();
            targetBody = hit.Attacker.LinkedThing.Body;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player") && curVirgoState == VirgoState.Waiting)
        {
            curVirgoState = VirgoState.Attacking;
            Agent.speed = trapSpeed;
        }
    }

    void OnAttackHit(HitInfos hit)
    {
        if (hit.AttackObject == grabAttack)
        {
            curVirgoState = VirgoState.AttackSucceed;
            curAttackState = AttackState.Attacking;

            targetBody = hit.Attacked.LinkedThing.Body;
            Visual.LookAt(targetBody.position.SetY(Body.position.y));

            Destroy(grabAttack.gameObject);
            Destroy(linkedVirgo.Find("TrapTrigger").gameObject);
        }
    }

    void GiveUpVirgo()
    {
        Utilities.SetLayerOfAllChildrens(linkedVirgo, 13);

        curVirgoState = VirgoState.Discovered;
        curAttackState = AttackState.Normal;

        linkedVirgo.SetParent(Game_NG.PhysicsHolder);

        Rigidbody virgoR = linkedVirgo.gameObject.AddComponent<Rigidbody>();
        virgoR.mass = virgoMass;
        virgoR.angularVelocity = Random.onUnitSphere * 5f;

        Agent.speed = moveSpeed;

        Agent.velocity = (Random.Range(0f, 1f) > 0.5f ? Visual.right : -Visual.right) * sideStepSpeed;

        if (grabAttack != null)
            Destroy(grabAttack.gameObject);

        if (linkedVirgo.Find("TrapTrigger") != null)
            Destroy(linkedVirgo.Find("TrapTrigger").gameObject);
    }

    private void OnDrawGizmos()
    {
        if (curVirgoState == VirgoState.Discovered)
            return;
        if (transform.Find("Body").Find("Virgo") == null)
            return;
        if (transform.Find("Body").Find("Virgo").Find("TrapTrigger") == null)
            return;

        BoxCollider coll = transform.Find("Body").Find("Virgo").Find("TrapTrigger").GetComponent<BoxCollider>();
        Gizmos.matrix = Matrix4x4.TRS(coll.transform.position, coll.transform.rotation, coll.transform.lossyScale);
        Gizmos.DrawWireCube(coll.center, coll.size);
    }

}