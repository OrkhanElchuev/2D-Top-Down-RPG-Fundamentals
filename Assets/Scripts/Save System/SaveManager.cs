using System.IO;
using Cinemachine;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    private string saveLocation;

    // Start is called once before the first execution of Update after the MonoBehaviour is created     
    void Start()
    {
        // Set the save location to a file named "saveData.json" in the persistent data path
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        LoadGame();
    }

    public void SaveGame()
    {
        SaveData saveData = new SaveData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundry = FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name
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
            player.transform.position = saveData.playerPos;
            FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D = GameObject.Find(saveData.mapBoundry).GetComponent<PolygonCollider2D>();
        }
        else
        {
            SaveGame();
            Debug.LogWarning("No save file found at " + saveLocation);
        }
    }
}
