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
    }

    public void SaveGame(SaveData data)
    {
        SaveData saveData = new SaveData
        {
            playerPos = GameObject.FindGameObjectWithTag("Player").transform.position,
            mapBoundry = FindAnyObjectByType<CinemachineConfiner>().m_BoundingShape2D.gameObject.name
        };
    }
}
