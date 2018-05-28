using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class p_InteractionController : p_Base 
{
    public enum e_InteractionState
    {
        None,
        Interacting
    }

    [SerializeField] e_InteractionState interactionState;
    new public e_InteractionState InteractionState { get { return interactionState; } }

    Interactable curInteractable;
    public Interactable CurrentInteractable { get { return curInteractable; } }

    bool CanInteract
    {
        get
        {
            return CurrentInteractable.BloodCost <= PB.CurBlood;
        }
    }

    float interactionProgress = 0f;

    private void Update()
    {
        if (curInteractable == null || EnemyLocked != null)
        {
            EndInteract();
            return;
        }

        if (C.Interact && CanInteract)
            interactionState = e_InteractionState.Interacting;
        if (C.StopInteract)
            EndInteract();

        if (interactionState == e_InteractionState.Interacting)
        {
            interactionProgress += C.InteractAxis;

            if (interactionProgress < -10)
            {
                if (CurrentInteractable.CanInteractBackward)
                {
                    EndInteract();
                    curInteractable.Interact(gameObject, false);
                }
                else
                    interactionProgress = -10;
            }
            else if (interactionProgress > 10)
            {
                if (CurrentInteractable.CanInteractForward)
                {
                    EndInteract();
                    curInteractable.Interact(gameObject, true);
                }
                else
                    interactionProgress = 10;
            }
        }
    }

    void EndInteract()
    {
        interactionProgress = 0;
        interactionState = e_InteractionState.None;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 15)
        {
            switch (other.tag)
            {
                case "LifeBonus":
                    GameObject.Find("HealthBar").GetComponent<RectTransform>().sizeDelta = new Vector2(40, 880);
                    GameObject.Find("HealthBar").GetComponent<RectTransform>().anchoredPosition = new Vector2(40, 460);
                    break;
            }
            Destroy(other.gameObject);
        }
    }

    void OnInteractableEnter(Interactable interactable)
    {
        curInteractable = interactable;
    }

    void OnInteractableExit(Interactable interactable)
    {
        if (interactable == curInteractable)
            curInteractable = null;
    }
}