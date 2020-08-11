using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** This script will hold static utility functions.
 */
public static class Utils
{
    /** Returns true or false randomly.
     */
    public static bool GetRandomBool()
    {
        return (Random.value > 0.5f);
    }

    public static Vector2 VectorToAbs(Vector2 vec)
    {
        return new Vector2(Mathf.Abs(vec.x), Mathf.Abs(vec.y));
    }

    public static Vector3 VectorToAbs(Vector3 vec)
    {
        return new Vector3(Mathf.Abs(vec.x), Mathf.Abs(vec.y), Mathf.Abs(vec.z));
    }
}
