using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public InventoryObject inventory;

    [SerializeField]
    public GameObject inventorySlotTemplate;

    public void RefreshInventoryItems()
    {
        foreach(InventorySlot slot in inventory.itemsContainer)
        {
            // TODO: Check how to change sprite and amount text.
            GameObject createdItem = Instantiate(inventorySlotTemplate);
            createdItem.transform.Find("Item Image").GetComponent<Image>().sprite = slot.item.itemSprite;
            createdItem.transform.parent = this.transform;
        }
    }
}
