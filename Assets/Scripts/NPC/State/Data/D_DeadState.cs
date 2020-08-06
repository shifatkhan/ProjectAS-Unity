using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New D_DeadState", menuName = "EntityNPC/State/Dead State")]
public class D_DeadState : ScriptableObject
{
    public GameObject deathChunkParticle;
    public GameObject deathBloodParticle;

    public Color deathChunkColor;
    public Color deathBloodColor;
}
