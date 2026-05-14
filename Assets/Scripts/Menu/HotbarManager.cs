using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class HotbarManager : MonoBehaviour
{
    public GameObject hotbarPanel;
    public GameObject slotPrefab;
    public int slotCount = 6;
    [SerializeField] private Transform equippedItemPosition;
    [SerializeField] private Transform equippedItemPositionUp;
    private GameObject currentlyEquippedItem;

    private ItemDictionary itemDictionary;
    private PlayerMovement playerMovement;

    private Key[] hotbarKeys;

    private void Awake()
    {
        itemDictionary = FindAnyObjectByType<ItemDictionary>();
        playerMovement = FindAnyObjectByType<PlayerMovement>();

        hotbarKeys = new Key[slotCount];
        for (int i = 0; i < slotCount; i++)
        {
            // Map hotbar keys to 1, 2, 3, etc., with the last slot mapped to 0
            hotbarKeys[i] = i < (slotCount - 1) ? (Key)((int)Key.Digit1 + i) : Key.Digit0;
        }
    }

    // Update is called once per frame
    void Update()
    {
        RefreshEquippedItemPosition();

        for (int i = 0; i < slotCount; i++)
        {
            if (Keyboard.current[hotbarKeys[i]].wasPressedThisFrame)
            {
               SelectItemInSlot(i);
            }
        }
    }

    // Method to select the item in the specified hotbar slot and equip it on the player
    void SelectItemInSlot(int slotIndex)
    {
        Slot slot = hotbarPanel.transform.GetChild(slotIndex).GetComponent<Slot>();
        if (slot.currentItem != null)
        {
            Item item = slot.currentItem.GetComponent<Item>();
            EquipItem(item);
        }
        else
        {
            // If slot is empty, unequip current item
            UnequipCurrentItem();
        }
    }

    private void EquipItem(Item item)
    {
        // Destroy currently equipped item if any
        UnequipCurrentItem();

        Transform targetPosition = GetCurrentEquipTransform();
        if (targetPosition != null && itemDictionary != null)
        {
            GameObject itemPrefab = itemDictionary.GetItemPrefab(item.ID);
            if (itemPrefab != null)
            {
                currentlyEquippedItem = Instantiate(itemPrefab, targetPosition);
                currentlyEquippedItem.transform.localPosition = Vector3.zero;
                currentlyEquippedItem.transform.localRotation = Quaternion.identity;
                currentlyEquippedItem.transform.localScale = Vector3.one;
            }
        }
    }

    private void RefreshEquippedItemPosition()
    {
        if (currentlyEquippedItem == null)
            return;

        Transform targetPosition = GetCurrentEquipTransform();
        if (targetPosition == null)
            return;

        if (currentlyEquippedItem.transform.parent != targetPosition)
        {
            currentlyEquippedItem.transform.SetParent(targetPosition, false);
            currentlyEquippedItem.transform.localPosition = Vector3.zero;
            currentlyEquippedItem.transform.localRotation = Quaternion.identity;
            currentlyEquippedItem.transform.localScale = Vector3.one;
        }
    }

    private Transform GetCurrentEquipTransform()
    {
        if (playerMovement != null && playerMovement.MoveInput.y > 0f && equippedItemPositionUp != null)
            return equippedItemPositionUp;

        return equippedItemPosition;
    }

    private void UnequipCurrentItem()
    {
        if (currentlyEquippedItem != null)
        {
            Destroy(currentlyEquippedItem);
            currentlyEquippedItem = null;
        }
    }

    public int GetEquippedItemID()
    {
        if (currentlyEquippedItem != null)
        {
            Item item = currentlyEquippedItem.GetComponent<Item>();
            return item != null ? item.ID : -1;
        }
        return -1;
    }

    public void SetEquippedItem(int itemID)
    {
        if (itemID == -1)
        {
            UnequipCurrentItem();
            return;
        }

        if (itemDictionary != null)
        {
            GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
            if (itemPrefab != null)
            {
                Item item = itemPrefab.GetComponent<Item>();
                if (item != null)
                {
                    EquipItem(item);
                }
            }
        }
    }

    // Method to retrieve the current inventory items and their slot indices for saving
    public List<InventorySaveData> GetHotbarItems()
    {
        List<InventorySaveData> invData = new List<InventorySaveData>();
        foreach(Transform slotTransform in hotbarPanel.transform)
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
    public void SetHotbarItems(List<InventorySaveData> hotbarSaveData)
    {
        // Clear existing items from the inventory
        foreach (Transform child in hotbarPanel.transform)
        {
            Destroy(child.gameObject);
        }

        // Recreate inventory slots 
        for (int i = 0; i < slotCount; i++)
        {
            Instantiate(slotPrefab, hotbarPanel.transform);
        }

        // Populate inventory slots with items based on the saved data
        foreach (InventorySaveData data in hotbarSaveData)
        {
            if (data.slotIndex < slotCount)
            {
                Slot slot = hotbarPanel.transform.GetChild(data.slotIndex).GetComponent<Slot>();
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
