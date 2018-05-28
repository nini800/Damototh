using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilitiesExtends {

	public static Vector3 FloorToInt(this Vector3 v)
    {
        return new Vector3(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }

    public static Vector3 SetX(this Vector3 v, float x)
    {
        return new Vector3(x, v.y, v.z);
    }
    public static Vector3 SetY(this Vector3 v, float y)
    {
        return new Vector3(v.x, y, v.z);
    }
    public static Vector3 SetZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, z);
    }
    public static Vector3 AddX(this Vector3 v, float x)
    {
        return new Vector3(v.x + x, v.y, v.z);
    }
    public static Vector3 AddY(this Vector3 v, float y)
    {
        return new Vector3(v.x, v.y + y, v.z);
    }
    public static Vector3 AddZ(this Vector3 v, float z)
    {
        return new Vector3(v.x, v.y, v.z + z);
    }


    public static Vector2 SetX(this Vector2 v, float x)
    {
        return new Vector2(x, v.y);
    }
    public static Vector2 SetY(this Vector2 v, float y)
    {
        return new Vector2(v.x, y);
    }
    
    public static Vector2 ToXZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }

    public static Vector3 ToXZ(this Vector2 v)
    {
        return new Vector3(v.x, 0, v.y);
    }

    public static Color SetA(this Color c, float a)
    {
        c.a = a;
        return c;
    }
    public static Color SetR(this Color c, float r)
    {
        c.r = r;
        return c;
    }
    public static Color SetG(this Color c, float g)
    {
        c.g = g;
        return c;
    }
    public static Color SetB(this Color c, float b)
    {
        c.b = b;
        return c;
    }
}
