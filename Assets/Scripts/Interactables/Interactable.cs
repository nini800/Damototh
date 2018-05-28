using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interactable : MonoBehaviour 
{
    [SerializeField] float bloodCost = 10f;
    [SerializeField] bool parentOnStart = false;
    [SerializeField] bool canInteractForward = true;
    [SerializeField] bool canInteractBackward = false;

    public float BloodCost { get { return bloodCost; } }
    public bool CanInteractForward { get { return canInteractForward; } }
    public bool CanInteractBackward { get { return canInteractBackward; } }

    Canvas interactCanvas;
    bool canDisplayCanvas = true;
    bool canvasEnabled = false;
    protected virtual void Awake()
    {
        interactCanvas = GetComponentInChildren<Canvas>();
        interactCanvas.enabled = false;
    }
    protected virtual void Update()
    {
        if (canDisplayCanvas)
        {
            interactCanvas.enabled = canvasEnabled;
            if (bloodCost > FindObjectOfType<p_PlayerBeing>().CurBlood)
                interactCanvas.GetComponentInChildren<Image>().color = new Color(1, 0, 0, 0.5f);
            else
                interactCanvas.GetComponentInChildren<Image>().color = interactCanvas.GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);

        }
        else
            interactCanvas.enabled = false;
    }
    Transform oParent;
    GameObject interacter;
    public virtual bool Interact(GameObject interacter, bool forward)
    {
        if (forward && !canInteractForward)
            return false;
        if (!forward && !canInteractBackward)
            return false;

        if (bloodCost > interacter.GetComponent<p_PlayerBeing>().CurBlood || !canDisplayCanvas)
            return false;
        else
            interacter.GetComponent<p_PlayerBeing>().AddBlood(-bloodCost);

        this.interacter = interacter;
        canDisplayCanvas = false;
        if (parentOnStart)
        {
            oParent = interacter.transform.parent;
            interacter.transform.SetParent(transform);
        }
        return true;
    }
    public virtual void EndInteract()
    {
        canDisplayCanvas = true;
        if (parentOnStart)
            interacter.transform.SetParent(oParent);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        canvasEnabled = true;
        other.GetComponentInParent<p_Base>().SendMessage("OnInteractableEnter", this, SendMessageOptions.DontRequireReceiver);
    }
    protected virtual void OnTriggerExit(Collider other)
    {
        canvasEnabled = false;
        other.GetComponentInParent<p_Base>().SendMessage("OnInteractableExit", this, SendMessageOptions.DontRequireReceiver);
    }
}