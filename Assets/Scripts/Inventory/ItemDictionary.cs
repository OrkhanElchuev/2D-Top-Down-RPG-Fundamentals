using System.Collections.Generic;
using UnityEngine;

public class ItemDictionary : MonoBehaviour
{
    public List<Item> itemPrefabs;
    private Dictionary<int, GameObject> itemDictionary;

    void Awake()
    {
        InitializeItemDictionary();
    }

    // Method to initialize the item dictionary
    private void InitializeItemDictionary()
    {
        itemDictionary = new Dictionary<int, GameObject>();

        for (int i = 0; i < itemPrefabs.Count; i++)
        {
            if (itemPrefabs[i] != null)
            {
                itemPrefabs[i].ID = i + 1; // Assign a unique ID based on the index
            }
        }

        foreach (Item item in itemPrefabs)
        {
            itemDictionary[item.ID] = item.gameObject;
        }
    }

    // Method to retrieve an item prefab by its ID
    public GameObject GetItemPrefab(int itemId)
    {
        itemDictionary.TryGetValue(itemId, out GameObject itemPrefab);
        if (itemPrefab == null)
        {
            Debug.LogWarning($"Item with ID {itemId} not found in the dictionary.");
        }
        return itemPrefab;
    }
}
