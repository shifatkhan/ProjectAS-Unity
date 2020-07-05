using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Scriptable object for a food item.
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory System/Items/Food")]
public class ItemFoodObject : ItemObject
{
    public int healValue;

    public void Awake()
    {
        type = ItemType.Food;
    }
}
