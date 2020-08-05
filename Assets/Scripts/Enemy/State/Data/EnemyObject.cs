using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyObject", menuName = "Enemy/Base Enemy")]
public class EnemyObject : ScriptableObject
{
    public float wallCheckDistance = 0.2f;
    public float groundCheckDistance = 0.4f;

    public float minAgroDistance = 3f;
    public float maxAgroDistance = 4f;

    public float closeRangeActionDistance = 1.8f;

    public float stunResistance = 3f; // How many hits until stunned.
    public float stunRecoveryTime = 2f;

    public GameObject hitParticle;
}
