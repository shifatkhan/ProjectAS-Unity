using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryObject inventory;

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
            // TODO: Check how to change sprite and amount text.
            GameObject createdItem = Instantiate(inventorySlotTemplate);
            createdItem.transform.Find("Item Image").GetComponent<Image>().sprite = itemSlot.item.itemPrefab.GetComponent<SpriteRenderer>().sprite;//itemSlot.item.itemSprite;
            
            // Don't show numbers if there's only 1.
            createdItem.transform.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = itemSlot.amount == 1 ? "" : itemSlot.amount.ToString();
            createdItem.transform.SetParent(transform.Find("ItemSlotsContainer"));
        }
    }
}
