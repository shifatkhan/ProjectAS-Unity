using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New ChargeStateObject", menuName = "Enemy/State/Charge State")]
public class ChargeStateObject : ScriptableObject
{
    public float chargeSpeed = 6f;

    public float chargeTime = 2f;
}
