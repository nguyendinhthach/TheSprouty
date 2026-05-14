// ──────────────────────────────────────────────
// TheSprouty | Fishing/FishSO.cs
// ScriptableObject defining data for a fish species.
// Extends ItemSO so fish can be added directly to InventoryManager.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "TheSprouty/Fishing/Fish")]
public class FishSO : ItemSO
{
    // maxStackSize inherited from ItemSO

    // ----------------------------------------------------------
    // Rarity & economy
    // ----------------------------------------------------------
    [Header("Rarity & Economy")]
    [Tooltip("Determines drop chance within the shadow's fish pool.")]
    public FishRarity rarity = FishRarity.Common;
    [Tooltip("Base sell price in gold.")]
    public int sellPrice = 10;

    // ----------------------------------------------------------
    // Behaviour
    // ----------------------------------------------------------
    [Header("Behaviour")]
    [Tooltip("Minimum seconds before the fish bites after reaching the bobber.")]
    [Min(0f)] public float minBiteDelay = 1f;

    [Tooltip("Maximum seconds before the fish bites after reaching the bobber.")]
    [Min(0f)] public float maxBiteDelay = 4f;

    // ----------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------
    /// <summary>Returns a random bite delay between minBiteDelay and maxBiteDelay.</summary>
    public float GetRandomBiteDelay() => Random.Range(minBiteDelay, maxBiteDelay);
}
