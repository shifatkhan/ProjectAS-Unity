using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerDetectedObject", menuName = "Enemy/State/Player Detected State")]
public class PlayerDetectedObject : ScriptableObject
{
    public float longRangeActionTime = 1.5f;   
}
