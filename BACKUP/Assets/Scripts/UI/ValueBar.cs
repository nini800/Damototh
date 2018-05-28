using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ValueBar : MonoBehaviour 
{
    public enum StatToLink
    {
        Health,
        Blood,
        Madness
    }

    [SerializeField] StatToLink stat;
    [SerializeField] [Range(0f, 0.5f)] float lerpSpeed;

    RectTransform fill;
    RectTransform rect;

    p_PlayerBeing pb;

    float curDisplayValue;

    protected void Awake ()
    {
        rect = GetComponent<RectTransform>();
        fill = transform.Find("Fill").GetComponent<RectTransform>();

        pb = FindObjectOfType<p_PlayerBeing>();

        switch (stat)
        {
            case StatToLink.Health:
                curDisplayValue = (pb.CurHealth / pb.MaxHealth);
                break;
            case StatToLink.Blood:
                curDisplayValue = (pb.CurBlood / pb.MaxBlood);
                break;
            case StatToLink.Madness:
                curDisplayValue = (pb.CurMadness / pb.MaxMadness);
                break;
        }
    }

    protected void Update()
    {

        switch (stat)
        {
            case StatToLink.Health:
                curDisplayValue = Utilities.Lerp(curDisplayValue, (pb.CurHealth / pb.MaxHealth), lerpSpeed);
                break;
            case StatToLink.Blood:
                curDisplayValue = Utilities.Lerp(curDisplayValue, (pb.CurBlood / pb.MaxBlood), lerpSpeed);
                break;
            case StatToLink.Madness:
                curDisplayValue = Utilities.Lerp(curDisplayValue, (pb.CurMadness / pb.MaxMadness), lerpSpeed);
                break;
        }

        SetFillPercent(curDisplayValue);
    }

    protected void SetFillPercent(float percent)
    {
        fill.sizeDelta = new Vector2(fill.sizeDelta.x, rect.sizeDelta.y * percent);
        fill.anchoredPosition = new Vector2(fill.anchoredPosition.x, fill.sizeDelta.y / 2f);
    }
	
}