using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New D_DodgeState", menuName = "EntityNPC/State/Dodge State")]
public class D_DodgeState : ScriptableObject, ISerializationCallbackReceiver
{
    public float dodgeSpeed = 10f;
    public float dodgeTime = 0.2f;
    public float dodgeCooldown = 2f;

    public Vector2 dodgeAngle;
    [NonSerialized] public Vector2 runtimeDodgeAngle;

    public void OnAfterDeserialize()
    {
        runtimeDodgeAngle = dodgeAngle;
    }

    public void OnBeforeSerialize() { }
}
