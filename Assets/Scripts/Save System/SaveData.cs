using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public Vector3 playerPos;
    public string mapBoundry; // Boundary name
    public List<InventorySaveData> inventorySaveData;
}
