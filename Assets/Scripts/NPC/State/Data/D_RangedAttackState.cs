using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New D_RangedAttackState", menuName = "EntityNPC/State/Ranged Attack State")]
public class D_RangedAttackState : ScriptableObject
{
    public GameObject projectile;
    public float projectileSpeed = 6f;
    public float projectileTravelDistance = 5f;
}
