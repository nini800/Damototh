using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MekanismInteractable : Interactable 
{
    [SerializeField] TransformMovement[] movementsOnInteract;
    [SerializeField] float interactCooldown = 2f;

    float lastInteractTime = -Mathf.Infinity;
    public override bool Interact(GameObject interacter, bool forward)
    {
        if (!base.Interact(interacter, forward))
            return false;

        float maxTime = 0;

        if (lastInteractTime + interactCooldown < Time.time)
        {
            lastInteractTime = Time.time;
            for (int i = 0; i < movementsOnInteract.Length; i++)
            {
                switch (forward)
                {
                    case true:
                    movementsOnInteract[i].InitiateMovement(false);
                        break;
                    case false:
                    movementsOnInteract[i].InitiateMovement(true);
                        break;
                }
                if (movementsOnInteract[i].TotalTime > maxTime)
                    maxTime = movementsOnInteract[i].TotalTime;
            }
        }

        Invoke("EndInteract", maxTime);
        return true;
    }

}