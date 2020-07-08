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
            GameObject createdItem = Instantiate(inventorySlotTemplate);
            createdItem.GetComponentInChildren<Image>().sprite = slot.item.itemPrefab.GetComponent<Sprite>();
            createdItem.transform.parent = this.transform;
        }
    }
}
