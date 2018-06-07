using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class e_Visual : e_Base 
{
    [SerializeField] protected float rotSpeed = 1f;
    MeleeWeaponTrail attackTrail;

    protected override void Awake()
    {
        base.Awake();
        attackTrail = Visual.GetComponentInChildren<MeleeWeaponTrail>();
    }

    protected virtual void Update ()
    {
        Visual.position = Body.position;
        Visual.rotation = Body.rotation;

        AnimationHandler();
    }

    protected virtual void AnimationHandler()
    {
        if (Agent.desiredVelocity.magnitude > 0.1f)
        {
            PlayAnimation("Run");
        }
        else
        {
            PlayAnimation("Idle");
        }
    }

    protected virtual void OnTakeHit()
    {
        StopCoroutine("TakeHitColorCoroutine");
        StartCoroutine("TakeHitColorCoroutine");
    }

    IEnumerator TakeHitColorCoroutine()
    {
        Material mat = Visual.GetComponentInChildren<Renderer>().material;
        Color oColor = mat.color;
        float count = 0f;
        while (count < 0.2f)
        {
            float progress = count / 0.2f;
            mat.color = oColor + new Color(0.1f * (1-progress), -0.1f * (1 - progress), -0.1f * (1 - progress));
            yield return new WaitForEndOfFrame();
            count += Time.deltaTime;
        }
        mat.color = oColor;
    }

    protected void PlayAnimation(string stateName, float crossFade = 0.2f, bool fromStart = false)
    {
        if ((anim.GetNextAnimatorStateInfo(0).IsName(stateName) || anim.GetCurrentAnimatorStateInfo(0).IsName(stateName)) && !fromStart)
            return;

        if (!fromStart)
            anim.CrossFadeInFixedTime(stateName, crossFade, 0);
        else
            anim.CrossFadeInFixedTime(stateName, crossFade, 0, 0, 0);
    }



    protected void OnAttackStart(EnemyAttackStats attack)
    {
        attackTrail.Emit = true;
    }

    protected void OnAttackStop(EnemyAttackStats attack)
    {
        attackTrail.Emit = false;
    }
}