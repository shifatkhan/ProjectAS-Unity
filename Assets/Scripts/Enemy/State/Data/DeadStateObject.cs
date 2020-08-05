using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New DeadStateObject", menuName = "Enemy/State/Dead State")]
public class DeadStateObject : ScriptableObject
{
    public GameObject deathChunkParticle;
    public GameObject deathBloodParticle;

    public Color deathChunkColor;
    public Color deathBloodColor;
}
