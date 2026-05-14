using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    private ItemDictionary itemDictionary;

    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public GameObject[] itemPrefabs;
    public int slotCount;

    private void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
    }

    // Method to retrieve the current inventory items and their slot indices for saving
    public List<InventorySaveData> GetInventoryItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                invData.Add(new InventorySaveData
                {
                    itemID = item.ID,
                    slotIndex = slotTransform.GetSiblingIndex()
                });
            }
        }
        return invData;
    }

    // Method to set inventory items based on saved data
    public void SetInventoryItems(List<InventorySaveData> inventorySaveData)
    {
        // Clear existing items from the inventory
        foreach (Transform child in inventoryPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Recreate inventory slots 
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, inventoryPanel.transform);
        }

        // Populate inventory slots with items based on the saved data
        foreach (InventorySaveData data in inventorySaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = inventoryPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
                GameObject itemPrefab = itemDictionary.GetItemPrefab(data.itemID);
                if (itemPrefab != null)
                {
                    GameObject item = Instantiate(itemPrefab, slot.transform, false);
                    item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    slot.currentItem = item;
                }
            }
        }
    }
}
