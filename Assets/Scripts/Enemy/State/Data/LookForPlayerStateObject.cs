using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LookForPlayerStateObject", menuName = "Enemy/State/Look for player State")]
public class LookForPlayerStateObject : ScriptableObject
{
    public float timeBetweenTurns = 1f;
    public int amountOfTurns = 3;
}
