using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public int ID;
    public string Name;

    // Method to handle item pickup logic, can be overridden for specific item types
    public virtual void PickUp()
    {
        Sprite itemIcon = GetComponent<Image>()?.sprite;
        if (ItemPickupUIManager.Instance != null)
        {
            ItemPickupUIManager.Instance.ShowItemPickupPopup(Name, itemIcon);
        }
    }
}
