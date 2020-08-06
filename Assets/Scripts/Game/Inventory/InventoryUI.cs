using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public D_Inventory inventory;

    [SerializeField]
    public GameObject inventorySlotTemplate;

    public void RefreshInventoryItems()
    {
        // Remove old items before rebuilding inventory.
        // This avoids duplicate items.
        foreach (Transform child in transform.Find("ItemSlotsContainer"))
        {
            if (child == inventorySlotTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach(InventorySlot itemSlot in inventory.itemsContainer)
        {
            GameObject createdItem = Instantiate(inventorySlotTemplate);
            createdItem.transform.Find("Item Image").GetComponent<Image>().sprite = itemSlot.item.itemPrefab.GetComponent<SpriteRenderer>().sprite;//itemSlot.item.itemSprite;
            
            // Don't show numbers if there's only 1.
            createdItem.transform.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = itemSlot.amount == 1 ? "" : itemSlot.amount.ToString();
            createdItem.transform.SetParent(transform.Find("ItemSlotsContainer"));
            
            // Add event for when Item is Right-clicked to remove item.
            createdItem.GetComponent<ClickableObject>().rightClick.AddListener(delegate { OnClickRemoveItem(itemSlot.item, itemSlot.amount); });
        }
    }

    /** Event to remove item when it is Right-clicked.
     */
    public void OnClickRemoveItem(D_Item item, int amount)
    {
        inventory.RemoveItem(item, amount);
        DropItem(item, amount);

        RefreshInventoryItems();
    }

    /** This will drop the item into the world space by creating a gameobject prefab
     * of said item.
     * This function will use a small factory design pattern to drop the appropriate item.
     */
    private void DropItem(D_Item item, int amount)
    {
        GameObject itemToDrop = item.itemPrefab;
        itemToDrop.GetComponent<Item>().amount = amount;

        Instantiate(itemToDrop, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);
    }
}
