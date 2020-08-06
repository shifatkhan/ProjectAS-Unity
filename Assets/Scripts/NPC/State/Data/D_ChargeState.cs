using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New D_ChargeState", menuName = "EntityNPC/State/Charge State")]
public class D_ChargeState : ScriptableObject
{
    public float chargeSpeed = 6f;

    public float chargeTime = 2f;
}
