using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** Scriptable object for the inventory of the player. This will hold
 * all the items in the player's posession.
 * @author ShifatKhan
 */
[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject
{
    public List<InventorySlot> itemsContainer = new List<InventorySlot>();

    /** Add a certain item to our inventory.
     * This will search if the item is already present in the inventory.
     * If it is, we add to the amount instead of creating a new slot.
     */
    public void AddItem(ItemObject item, int amount)
    {
        // TODO: Change List to Dictionnary for better searching.
        bool hasItem = false;

        // Search if Item is already present.
        for (int i = 0; i < itemsContainer.Count; i++)
        {
            if(itemsContainer[i].item == item)
            {
                itemsContainer[i].AddAmount(amount);
                hasItem = true;
                break;
            }
        }

        // Add new instance of item if it's not already in inventory.
        if (!hasItem)
        {
            itemsContainer.Add(new InventorySlot(item, amount));
        }
    }
}

/** Scriptable object for a slot in the inventory. This links the actual
 * item itself with the inventory.
 * @author ShifatKhan
 */
[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    public int amount;

    public InventorySlot(ItemObject item, int amount)
    {
        this.item = item;
        this.amount = amount;
    }

    /** Add a certain amount to this item.
     */
    public void AddAmount(int amountToAdd)
    {
        amount += amountToAdd;
    }
}
