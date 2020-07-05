using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Scriptable object for holding data about an Item. Can be 
 * Inherited from for various item types.
 * @author ShifatKhan
 */
 public enum ItemType
{
    Food,
    Equipment,
    Default
}

public class ItemObject : ScriptableObject
{
    public GameObject itemPrefab;
    public ItemType type;

    [TextArea(15,20)]
    public string description; // A short paragraph describing the item.
}
