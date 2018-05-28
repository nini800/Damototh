using UnityEngine;
using System.Collections;

public class WheelSlipAddon : MonoBehaviour {

    WheelCollider item;

    float forwardExtremumValue;
    float sideWayExtremumValue;

    // Use this for initialization
    void Start ()
    {
        item = GetComponent<WheelCollider>();
        forwardExtremumValue = item.forwardFriction.extremumValue;
        sideWayExtremumValue = item.sidewaysFriction.extremumValue;
    }
	
	// Update is called once per frame
	void Update ()
    {
        //Slip handler        
        WheelHit hit;
        if (item.GetGroundHit(out hit))
        {
            if (Mathf.Abs(hit.forwardSlip) > item.forwardFriction.extremumSlip)
            {
                float proportionOfValueFactor; //We will multiply value with this factor depending on how much we are sliping
                if (item.forwardFriction.asymptoteSlip - Mathf.Abs(hit.forwardSlip) > 0) //If its not negative
                    proportionOfValueFactor = (item.forwardFriction.asymptoteSlip - Mathf.Abs(hit.forwardSlip)) / (item.forwardFriction.asymptoteSlip - item.forwardFriction.extremumSlip); //We get how much between 0 & 1
                else
                    proportionOfValueFactor = 0;

                WheelFrictionCurve curve = new WheelFrictionCurve();
                curve.extremumSlip = item.sidewaysFriction.extremumSlip;
                curve.extremumValue = (sideWayExtremumValue - item.sidewaysFriction.asymptoteValue) *proportionOfValueFactor + item.sidewaysFriction.asymptoteValue;
                curve.asymptoteSlip = item.sidewaysFriction.asymptoteSlip;
                curve.asymptoteValue = item.sidewaysFriction.asymptoteValue;
                curve.stiffness = item.sidewaysFriction.stiffness;
                item.sidewaysFriction = curve;
            }
            else
            {
                WheelFrictionCurve curve = new WheelFrictionCurve();
                curve.extremumSlip = item.sidewaysFriction.extremumSlip;
                curve.extremumValue = sideWayExtremumValue;
                curve.asymptoteSlip = item.sidewaysFriction.asymptoteSlip;
                curve.asymptoteValue = item.sidewaysFriction.asymptoteValue;
                curve.stiffness = item.sidewaysFriction.stiffness;
                item.sidewaysFriction = curve;
            }
            if (Mathf.Abs(hit.sidewaysSlip) > item.sidewaysFriction.extremumSlip)//Si on dépasse la limite d'adhérence
            {
                float proportionOfValueFactor; //We will multiply value with this factor depending on how much we are sliping
                if (item.sidewaysFriction.asymptoteSlip - Mathf.Abs(hit.sidewaysSlip) > 0) //If its not negative
                    proportionOfValueFactor = (item.sidewaysFriction.asymptoteSlip - Mathf.Abs(hit.sidewaysSlip)) / (item.sidewaysFriction.asymptoteSlip - item.sidewaysFriction.extremumSlip); //We get how much between 0 & 1
                else
                    proportionOfValueFactor = 0;

                WheelFrictionCurve curve = new WheelFrictionCurve();
                curve.extremumSlip = item.forwardFriction.extremumSlip;
                curve.extremumValue = (forwardExtremumValue - item.forwardFriction.asymptoteValue) * proportionOfValueFactor + item.forwardFriction.asymptoteValue;
                curve.asymptoteSlip = item.forwardFriction.asymptoteSlip;
                curve.asymptoteValue = item.forwardFriction.asymptoteValue;
                curve.stiffness = item.forwardFriction.stiffness;
                item.forwardFriction = curve;
            }
            else
            {
                WheelFrictionCurve curve = new WheelFrictionCurve();
                curve.extremumSlip = item.forwardFriction.extremumSlip;
                curve.extremumValue = forwardExtremumValue;
                curve.asymptoteSlip = item.forwardFriction.asymptoteSlip;
                curve.asymptoteValue = item.forwardFriction.asymptoteValue;
                curve.stiffness = item.forwardFriction.stiffness;
                item.forwardFriction = curve;
            }
        }

        
    }
}
