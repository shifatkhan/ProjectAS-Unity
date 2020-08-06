using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Dialogue/Character")]
public class D_Character : ScriptableObject
{
    public string fullName;
    public Sprite portrait;
}
