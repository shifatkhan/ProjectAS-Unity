using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New EnemyObject", menuName = "Enemy/Base Enemy")]
public class EnemyObject : ScriptableObject
{
    public float wallCheckDistance = 0.2f;
    public float groundCheckDistance = 0.4f;
}
