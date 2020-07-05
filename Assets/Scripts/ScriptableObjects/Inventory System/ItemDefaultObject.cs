using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Scriptable object a default item.
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Default Item", menuName = "Inventory System/Items/Default")]
public class ItemDefaultObject : ItemObject
{
    public void Awake()
    {
        type = ItemType.Default;
    }
}
