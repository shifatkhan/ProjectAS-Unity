using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Scriptable object for an equipment item.
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Equipment Item", menuName = "Inventory System/Items/Equipment")]
public class D_ItemEquipment : D_Item
{
    public float attackBonus;
    public float defenceBonus;

    public void Awake()
    {
        type = ItemType.Equipment;
    }
}
