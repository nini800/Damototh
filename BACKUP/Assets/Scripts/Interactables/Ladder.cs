using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    public enum LadderState
    {
        Retracted,
        Extended
    }

    [SerializeField] LadderState ladderState;
    [SerializeField] float retractedHeight;
    [SerializeField] float extendedHeight;
    [SerializeField] Vector2 widthDepth = new Vector2(0.5f, 0.5f);
    [SerializeField] Transform climbTarget;

    BoxCollider ladderHitbox;
    float currentHeight;
    float targetHeight { get { return ladderState == LadderState.Extended ? extendedHeight : retractedHeight; } }

    public Vector3 Center { get { return ladderHitbox.transform.forward * ladderHitbox.center.z * 0.75f + ladderHitbox.transform.position; } }
    public Vector3 ClimbPosition { get { return climbTarget.position; } }
    public float MaxWorldHeight { get { return transform.position.y; } }
    public float MinWorldHeight { get { return transform.position.y - currentHeight; } }

    void Awake ()
    {
        ladderHitbox = GetComponentInChildren<BoxCollider>();
        currentHeight = targetHeight;
    }

    private void Update()
    {
        currentHeight = Utilities.Lerp(currentHeight, targetHeight, 0.1f);
        ActualizeHitbox();
    }

    /// <summary>
    /// Return true if height > ladderMaxHeight, return false if height < ladderMinheight else return null
    /// Height is in worldPosition
    /// </summary>
    public bool? TestHeight(float height)
    {
        if (height > MaxWorldHeight)
            return true;
        else if (height < MinWorldHeight)
            return false;
        else
            return null;
    }

    void ActualizeHitbox()
    {
        ladderHitbox.size = new Vector3(widthDepth.x, currentHeight, widthDepth.y);
        ladderHitbox.center = new Vector3(0, -ladderHitbox.size.y * 0.5f, ladderHitbox.size.z * 0.5f);

        if (transform.Find("TestVisual"))
        {
            transform.Find("TestVisual").localScale = new Vector3(1, ladderHitbox.size.y, 0.12f);
            transform.Find("TestVisual").localPosition = new Vector3(0, -ladderHitbox.size.y * 0.5f, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        LivingBeing beingEntered;

        if (beingEntered = other.GetComponentInParent<LivingBeing>())
            beingEntered.SendMessage("OnLadderEnter", this, SendMessageOptions.DontRequireReceiver);
    }

    private void OnDrawGizmos()
    {
        if (climbTarget != null)
            Gizmos.DrawSphere(climbTarget.position, 0.1f);
    }

    private void OnValidate()
    {
        if (ladderHitbox == null)
            Awake();

        currentHeight = targetHeight;
        ActualizeHitbox();
    }
}
