using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "TheSprouty/Items/Item")]
public class ItemSO : BaseItemSO
{
    [Header("Item-Specific Data")]
    [Tooltip("Stackable max amount in inventory.")]
    public int maxStackSize = 99;
}
