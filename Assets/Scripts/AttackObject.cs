using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackObject : MonoBehaviour 
{
    protected Collider hitbox;

    protected HitInfos hitInfos;
    protected AttackStats stats { get { return hitInfos.AttackStats; } }
    protected LivingBeing AttackOwner { get { return hitInfos.Attacker; } }

    protected List<LivingBeing> alreadyHit = new List<LivingBeing>();

    public AttackStats.AttackWeightType Weight { get; protected set; }

    int maxHitTimes = 0;

    public void Initialize(HitInfos hitInfos, AttackStats.AttackWeightType weight = AttackStats.AttackWeightType.SameAsStats)
    {
        if (weight != AttackStats.AttackWeightType.SameAsStats)
            Weight = weight;
        else
            Weight = hitInfos.AttackStats.weight;

        this.hitInfos = new HitInfos(hitInfos, this);
        maxHitTimes = hitInfos.AttackStats.maxHitTimes;
    }

    protected void Start ()
    {
        hitbox = GetComponentInChildren<Collider>();

        if (stats.hitboxLifeTime > 0)
            Destroy(gameObject, stats.hitboxLifeTime);
	}

    private void Update()
    {
        if (stats.followCaster)
        {
            transform.position = AttackOwner.LinkedThing.Body.position;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        LivingBeing thingHit = other.GetComponentInParent<LivingBeing>();

        RaycastHit hit;
        Ray ray = new Ray(transform.position, (other.transform.position - transform.position));

        if (thingHit != null)
        {
            if (alreadyHit.Contains(thingHit))
                return;
            else
                alreadyHit.Add(thingHit);


            hitInfos = new HitInfos(hitInfos, ray.direction, false);

            if (AttackOwner is p_PlayerBeing)
            {
                Physics.Raycast(ray, out hit, 100f, Game_NG.EnemyLayerMask);
                hitInfos = new HitInfos(hitInfos, hit.point, true);

                float blood, madness;
                ((e_EnemyBeing)thingHit).TakeHit(hitInfos, out blood, out madness);

                hitInfos = new HitInfos(hitInfos, blood, madness);
            }
            else if (AttackOwner is e_EnemyBeing)
            {
                Physics.Raycast(ray, out hit, 100f, Game_NG.PlayerLayerMask);
                hitInfos = new HitInfos(hitInfos, hit.point, true);

                ((p_PlayerBeing)thingHit).TakeHit(hitInfos);
            }

            hitInfos = new HitInfos(hitInfos, thingHit);

            AttackOwner.SendMessage("OnAttackHit", new HitInfos(hitInfos, thingHit), SendMessageOptions.DontRequireReceiver);
            maxHitTimes--;

            if (maxHitTimes <= 0)
                Destroy(gameObject);


            FX.SpawnHitFX(hitInfos.HitPoint, -hitInfos.Direction, thingHit.transform.parent);
        }
        else
        {
            Breakable breakableHit = other.GetComponentInParent<Breakable>();
            Physics.Raycast(ray, out hit, 100f, Game_NG.AttackableLayerMask);


            if (breakableHit != null)
            {
                breakableHit.TakeHit(new HitInfos(hitInfos, hit.point, ray.direction));
            }
        }
        
    }
}