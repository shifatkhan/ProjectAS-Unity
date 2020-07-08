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
            createdItem.transform.Find("Item Image").GetComponent<Image>().sprite = itemSlot.item.itemSprite;
            createdItem.transform.Find("Amount Text").GetComponent<TextMeshProUGUI>().text = itemSlot.amount.ToString();
            createdItem.transform.parent = transform.Find("ItemSlotsContainer");
        }
    }
}
