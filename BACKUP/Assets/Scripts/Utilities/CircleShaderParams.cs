using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CircleShaderParams
{
    public Color color;
    public float radius;
    public float width;
    public float fill = 1;
    public float startAngle;
    public float antiAliasingDistance = 0.000371f;
    public float antiAliasingLinearity = 1f;

    CircleShaderParams(Color color, float radius, float width, float fill, float startAngle, float antiAliasingDistance, float antiAliasingLinearity)
    {
        this.color = color;
        this.radius = radius;
        this.width = width;
        this.fill = fill;
        this.startAngle = startAngle;
        this.antiAliasingDistance = antiAliasingDistance;
        this.antiAliasingLinearity = antiAliasingLinearity;
    }

    public static CircleShaderParams Lerp(CircleShaderParams a, CircleShaderParams b, float t)
    {
        return new CircleShaderParams
        (
            Color.Lerp(a.color, b.color, t),
            Utilities.Lerp(a.radius, b.radius, t),
            Utilities.Lerp(a.width, b.width, t),
            Utilities.Lerp(a.fill, b.fill, t),
            Utilities.Lerp(a.startAngle, b.startAngle, t),
            Utilities.Lerp(a.antiAliasingDistance, b.antiAliasingDistance, t),
            Utilities.Lerp(a.antiAliasingLinearity, b.antiAliasingLinearity, t)
        );
    }
}
