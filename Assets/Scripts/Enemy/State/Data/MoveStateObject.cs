using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MoveStateObject", menuName = "Enemy/State/Move State")]
public class MoveStateObject : ScriptableObject
{
    public float movementSpeed = 3f;
}
