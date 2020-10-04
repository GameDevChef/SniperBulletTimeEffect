using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extentions
{
    public static Vector3 WithX(this Vector3 vector3, float x)
    {
        return new Vector3(x, vector3.y, vector3.z);
    }

    public static Vector3 WithY(this Vector3 vector3, float y)
    {
        return new Vector3(vector3.x, y, vector3.z);
    }

    public static Vector3 WithZ(this Vector3 vector3, float z)
    {
        return new Vector3(vector3.x, vector3.y, z);
    }
}
