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
        SaveData saveData = new SaveData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundry = FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name,
            inventorySaveData = inventoryManager.GetInventoryItems(),
            hotbarSaveData = hotbarManager.GetHotbarItems()
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
    }

    // Load the game data from the save file
    public void LoadGame()
    {
        if (File.Exists(saveLocation))
        {
            // Read the save data from the file and apply it to the game
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                player.transform.position = saveData.playerPos;
            
            FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find(saveData.mapBoundry).GetComponent<PolygonCollider2D>();
            inventoryManager.SetInventoryItems(saveData.inventorySaveData);
            hotbarManager.SetHotbarItems(saveData.hotbarSaveData);
        }
        else
        {
            SaveGame();
            Debug.LogWarning("No save file found at " + saveLocation);
        }
    }
}
