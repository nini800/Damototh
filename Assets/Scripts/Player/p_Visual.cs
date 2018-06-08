using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_Visual : p_Base 
{
    [SerializeField] float rotSpeed = 5f;
    [SerializeField] float stepSpeed = 0.05f;
    float curAngle = 0f;
    float stepOffset = 0f;

    MeleeWeaponTrail attackTrail;

    protected override void Awake()
    {
        base.Awake();

        attackTrail = Visual.GetComponentInChildren<MeleeWeaponTrail>();
        //attackTrail.Emit = false;
    }

    protected void FixedUpdate()
    {
        stepOffset = Utilities.SubtractFloat(stepOffset, stepSpeed * Time.fixedDeltaTime * 50f, 0, true);
    }

    protected void Update ()
    {
        Visual.position = Body.position + Vector3.up * stepOffset;

        RotationHandler();
        AnimationHandler();
        ToMoveSomewhereElse();
    }

    protected void RotationHandler()
    {
        if (MovementState != p_MovementController.e_MovementState.Climbing)
        {
            if ((PB.CurLivingState & (LivingBeing.LivingState.Stunned | LivingBeing.LivingState.Dead)) == 0)
            {
                if (MC.Dashing)
                {
                    //Visual.LookAt(Visual.position + R.velocity.SetY(0));
                }
                else
                {
                    if (EnemyLocked == null)
                    {
                        if (MC.MoveDirection.sqrMagnitude > 0 && (AC.attackState & (p_AttackController.e_AttackState.None | p_AttackController.e_AttackState.Recovering)) != 0)
                        {
                            float moveAngle = Utilities.GetAngle(MC.MoveDirection.ToXZ()) + 90f;
                            curAngle = Utilities.ChangeToward(curAngle, moveAngle, rotSpeed * Utilities.Delta, true);
                            Visual.localEulerAngles = Visual.localEulerAngles.SetY(curAngle);
                        }
                    }
                    else
                    {
                        if (AttackState == p_AttackController.e_AttackState.None || AttackState == p_AttackController.e_AttackState.Recovering)
                        {
                            Visual.localRotation = Quaternion.RotateTowards(Visual.localRotation, Quaternion.LookRotation(EnemyLocked.Body.position.SetY(Visual.position.y) - Visual.position), rotSpeed * Utilities.Delta);
                            curAngle = Visual.localEulerAngles.y;
                        }
                    }
                }
            }
        }
        else
        {
            Visual.localRotation = Quaternion.RotateTowards(Visual.localRotation, Quaternion.LookRotation(-MC.CurrentLadder.transform.forward), rotSpeed * Utilities.Delta);
            curAngle = Visual.localEulerAngles.y;
        }
        
    }
    protected void AnimationHandler()
    {
        if (MovementState != p_MovementController.e_MovementState.Climbing)
        {
            float curSpeed = MC.MoveInput.magnitude;

            if (PB.CurLivingState == LivingBeing.LivingState.Stunned)
                PlayAnimation("Idle", 0.4f);
            else if (AC.CurrentAttack != null && (AttackState != p_AttackController.e_AttackState.Recovering))
            {
                switch (AC.CurrentAttack.Name)
                {
                    case "Light_1":
                        PlayAnimation("lightattack_simple_A", 0.1f);
                        break;
                    case "Light_2":
                        PlayAnimation("lightattack_simple_B", 0.1f);
                        break;
                    case "Heavy_1":
                        PlayAnimation("Combo_Loud1", 0.1f);
                        break;
                    case "Heavy_2":
                        PlayAnimation("Combo_Loud2", 0.1f);
                        break;
                    case "Sprinting_Heavy_1":
                        PlayAnimation("Attaque3", 0.3f);
                        break;
                }
            }
            else if (AttackState == p_AttackController.e_AttackState.Mutilating || AttackState == p_AttackController.e_AttackState.MutilateRecover)
                PlayAnimation("Mutilation", 0.3f);
            else if (!Dashing && Grounded)
            {
                if (!EnemyLocked)
                {
                    if (curSpeed <= 0)
                        PlayAnimation("Idle", 0.4f);
                    else if (curSpeed <= 0.5f && !Sprinting)
                        PlayAnimation("Walk", 0.4f);
                    else if (curSpeed > 0.5f && !Sprinting)
                        PlayAnimation("Run", 0.4f);
                    else
                        PlayAnimation("Sprint", 0.4f);
                }
                else
                {
                    switch (MC.MovementState)
                    {
                        case p_MovementController.e_MovementState.Standing:
                            PlayAnimation("Idle_Fight", 0.2f);
                            break;
                        case p_MovementController.e_MovementState.WalkLeft:
                            PlayAnimation("Run_Fight_Left", 0.4f);
                            break;
                        case p_MovementController.e_MovementState.WalkRight:
                            PlayAnimation("Run_Fight_Right", 0.4f);
                            break;
                        case p_MovementController.e_MovementState.WalkForward:
                            PlayAnimation("Run_Fight_Forward", 0.4f);
                            break;
                        case p_MovementController.e_MovementState.WalkBackward:
                            PlayAnimation("Run_Fight_Backward", 0.4f);
                            break;
                        case p_MovementController.e_MovementState.RunLeft:
                            PlayAnimation("Run_Fight_Left", 0.2f);
                            break;
                        case p_MovementController.e_MovementState.RunRight:
                            PlayAnimation("Run_Fight_Right", 0.2f);
                            break;
                        case p_MovementController.e_MovementState.RunForward:
                            PlayAnimation("Run_Fight_Forward", 0.2f);
                            break;
                        case p_MovementController.e_MovementState.RunBackward:
                            PlayAnimation("Run_Fight_Backward", 0.2f);
                            break;
                        case p_MovementController.e_MovementState.SprintingBackward:
                        case p_MovementController.e_MovementState.SprintingForward:
                        case p_MovementController.e_MovementState.SprintingLeft:
                        case p_MovementController.e_MovementState.SprintingRight:
                            PlayAnimation("Sprint", 0.4f);
                            break;
                        case p_MovementController.e_MovementState.InAir:
                            break;
                        case p_MovementController.e_MovementState.Climbing:
                            break;
                        default:
                            break;
                    }
                }
            }
            else if (Dashing)
            {
                switch (MovementState)
                {
                    case p_MovementController.e_MovementState.DashingLeft:
                        PlayAnimation("Dash_Left", 0.15f);
                        break;
                    case p_MovementController.e_MovementState.DashingRight:
                        PlayAnimation("Dash_Right", 0.15f);
                        break;
                    case p_MovementController.e_MovementState.DashingForward:
                        PlayAnimation("Dash_Left", 0.15f);
                        break;
                    case p_MovementController.e_MovementState.DashingBackward:
                        PlayAnimation("Dash_Back", 0.15f);
                        break;
                }
            }
        }
        else
        {
            PlayAnimation("Idle", 0.4f);
        }


        anim.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 0.5f);
        anim.SetIKHintPosition(AvatarIKHint.RightElbow, Vector3.zero);

    }
    protected void ToMoveSomewhereElse()
    {
        //TO MOVE SOMEWHERE ELSE
        if (EnemyLocked == null)
        {
            LockArrow.gameObject.SetActive(false);
        }
        else
        {
            LockArrow.gameObject.SetActive(true);
            LockArrow.position = EnemyLocked.Body.position + Vector3.up.SetY(EnemyLocked.BodyColl.height + 1f);
        }
    }

    void OnCastAttack(PlayerAttackStats attack)
    {
        if (!EnemyLocked)
            ForceRotation();
    }
    public void ForceRotation()
    {
        if (MC.MoveDirection.sqrMagnitude == 0)
            return;

        float moveAngle = Utilities.GetAngle(MC.MoveDirection.ToXZ()) + 90f;
        curAngle = moveAngle;
        Visual.localEulerAngles = Visual.localEulerAngles.SetY(moveAngle);
    }
    public void TellStepOffset(float offset)
    {
        stepOffset -= offset;
    }


    void PlayAnimation(string stateName, float crossFade = 0.2f, bool fromStart = false)
    {
        if ((anim.GetNextAnimatorStateInfo(0).IsName(stateName) || anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)) && !fromStart)
            return;

        if (!fromStart)
            anim.CrossFadeInFixedTime(stateName, crossFade, 0);
        else
            anim.CrossFadeInFixedTime(stateName, crossFade, 0, 0, 0);
    }

    void OnJump()
    {
        PlayAnimation("Jump", 0.1f, true);
        ForceRotation();
    }

    void OnAttackStart(PlayerAttackStats attack)
    {
        attackTrail.Emit = true;
    }

    void OnAttackStop(PlayerAttackStats attack)
    {
        attackTrail.Emit = false;
    }
}