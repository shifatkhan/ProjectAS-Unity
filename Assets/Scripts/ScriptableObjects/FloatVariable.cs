using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Variables/FloatVariable")]
public class FloatVariable : ScriptableObject, ISerializationCallbackReceiver
{
    public float InitialValue;

    [System.NonSerialized]
    public float RuntimeValue;

    public void OnAfterDeserialize()
    {
        RuntimeValue = InitialValue;
    }

    public void OnBeforeSerialize() { }
}
