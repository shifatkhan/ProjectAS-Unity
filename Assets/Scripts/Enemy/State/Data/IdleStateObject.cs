using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New IdleStateObject", menuName = "Enemy/State/Idle State")]
public class IdleStateObject : ScriptableObject
{
    public float minIdleTime = 1f;
    public float maxIdleTime = 2f;
}
