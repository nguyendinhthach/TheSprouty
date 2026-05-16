using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "TheSprouty/Items/Item")]
public class ItemSO : BaseItemSO
{
    [Header("Item-Specific Data")]
    [Tooltip("Stackable max amount in inventory.")]
    public int maxStackSize = 99;

    [Header("Economy")]
    [Tooltip("Gold player receives when selling this item. 0 = cannot be sold.")]
    public int sellValue = 0;
}
