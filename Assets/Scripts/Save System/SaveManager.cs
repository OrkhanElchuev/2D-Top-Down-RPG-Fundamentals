using System.IO;
using Cinemachine;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string saveLocation;
    private InventoryManager inventoryManager;
    private HotbarManager hotbarManager;

    private void Awake()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");
        inventoryManager = FindAnyObjectByType<InventoryManager>();
        hotbarManager = FindAnyObjectByType<HotbarManager>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created     
    void Start()
    {
        LoadGame();
    }

    public void SaveGame()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("SaveManager: Player with tag 'Player' not found. Cannot save game.");
            return;
        }

        CinemachineConfiner confiner = FindAnyObjectByType<CinemachineConfiner>();
        if (confiner == null || confiner.m_BoundingShape2D == null)
        {
            Debug.LogError("SaveManager: CinemachineConfiner or its bounding shape was not found. Cannot save map boundary.");
            return;
        }

        inventoryManager ??= FindAnyObjectByType<InventoryManager>();
        hotbarManager ??= FindAnyObjectByType<HotbarManager>();

        SaveData saveData = new SaveData
        {
            playerPos = player.transform.position,
            mapBoundry = confiner.m_BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryManager != null ? inventoryManager.GetInventoryItems() : new System.Collections.Generic.List<InventorySaveData>(),
            hotbarSaveData = hotbarManager != null ? hotbarManager.GetHotbarItems() : new System.Collections.Generic.List<InventorySaveData>(),
            equippedItemID = hotbarManager != null ? hotbarManager.GetEquippedItemID() : -1
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData, true));
        Debug.Log($"SaveManager: Game saved to {saveLocation}");
    }

    // Load the game data from the save file
    public void LoadGame()
    {
        inventoryManager ??= FindAnyObjectByType<InventoryManager>();
        hotbarManager ??= FindAnyObjectByType<HotbarManager>();

        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            if (saveData == null)
            {
                Debug.LogWarning($"SaveManager: Save file at {saveLocation} could not be parsed. Creating a fresh save.");
                SaveGame();
                return;
            }

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = saveData.playerPos;

            if (!string.IsNullOrEmpty(saveData.mapBoundry))
            {
                GameObject boundary = GameObject.Find(saveData.mapBoundry);
                CinemachineConfiner confiner = FindAnyObjectByType<CinemachineConfiner>();
                if (boundary != null && confiner != null)
                {
                    PolygonCollider2D collider = boundary.GetComponent<PolygonCollider2D>();
                    if (collider != null)
                        confiner.m_BoundingShape2D = collider;
                }
            }

            if (inventoryManager != null)
                inventoryManager.SetInventoryItems(saveData.inventorySaveData);
            else
                Debug.LogWarning("SaveManager: InventoryManager not found when loading saved inventory.");

            if (hotbarManager != null)
            {
                hotbarManager.SetHotbarItems(saveData.hotbarSaveData);
                hotbarManager.SetEquippedItem(saveData.equippedItemID);
            }
            else
                Debug.LogWarning("SaveManager: HotbarManager not found when loading saved hotbar.");
        }
        else
        {
            SaveGame();
            Debug.LogWarning("No save file found at " + saveLocation);
        }
    }

    private void OnApplicationQuit()
    {
        SaveGame();
    }
}
