using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_NG : MonoBehaviour
{
    public static Game_NG Instance { get; protected set; }

    public static Transform EnemyHolder { get { return Instance.enemyHolder; } }
    public static Transform AttackHitboxHolder { get { return Instance.attackHitboxHolder; } }
    public static Transform PhysicsHolder { get { return Instance.physicsHolder; } }

    public static LayerMask GroundLayerMask { get { return Instance.groundLayerMask; } }
    public static LayerMask PlayerLayerMask { get { return Instance.playerLayerMask; } }
    public static LayerMask EnemyLayerMask { get { return Instance.enemyLayerMask; } }
    public static LayerMask AttackableLayerMask { get { return Instance.attackableLayerMask; } }
    public static LayerMask BeingLayerMask { get { return Instance.beingLayerMask; } }

    [Header("Holders")]
    [SerializeField] Transform enemyHolder;
    [SerializeField] Transform attackHitboxHolder;
    [SerializeField] Transform physicsHolder;

    [Header("Layer Masks")]
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] LayerMask playerLayerMask;
    [SerializeField] LayerMask enemyLayerMask;
    [SerializeField] LayerMask attackableLayerMask;
    [SerializeField] LayerMask beingLayerMask;


    protected void Awake()
    {
        Instance = this;
    }
}
