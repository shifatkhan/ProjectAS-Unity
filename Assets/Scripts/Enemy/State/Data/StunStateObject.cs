using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New StunStateObject", menuName = "Enemy/State/Stun State")]
public class StunStateObject : ScriptableObject
{
    public float stunTime = 3f;

    public float stunKnockbackTime = 0.2f;
    public float stunKnockbackSpeed = 20f;
    public Vector2 stunKnockbackAngle;
}
