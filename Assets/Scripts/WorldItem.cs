// ──────────────────────────────────────────────
// TheSprouty | Scripts/WorldItem.cs
// Represents a physical item in the world. Implements ICollectable
// so ItemPickup can trigger collection without knowing item details.
// ──────────────────────────────────────────────
using UnityEngine;

public class WorldItem : MonoBehaviour, ICollectable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private ItemSO itemSO;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public string ObjectName => itemSO != null ? itemSO.itemName : gameObject.name;

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------
    public ItemSO GetItemSO() => itemSO;

    /// <summary>
    /// Called by ItemPickup when the player collects this item.
    /// Replace Debug.Log with InventoryManager call when inventory is ready.
    /// </summary>
    public void OnCollected()
    {
        Debug.Log($"Collect: {ObjectName}");

        if (itemSO != null)
        {
            InventoryManager.Instance.AddItem(itemSO, 1);
        }
    }
}
