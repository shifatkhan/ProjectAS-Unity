using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "ScriptableObject/Dialogue/CharacterObject")]
public class CharacterObject : ScriptableObject
{
    public string fullName;
    public Sprite portrait;
}
