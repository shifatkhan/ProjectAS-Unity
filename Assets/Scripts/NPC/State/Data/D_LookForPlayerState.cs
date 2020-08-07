using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LookForPlayerStateObject", menuName = "EntityNPC/State/Look for player State")]
public class D_LookForPlayerState : ScriptableObject
{
    public float timeBetweenTurns = 1f;
    public int amountOfTurns = 2;
}
