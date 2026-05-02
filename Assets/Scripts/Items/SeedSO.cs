// ──────────────────────────────────────────────
// TheSprouty | Items/SeedSO.cs
// ScriptableObject for seed items. Extends ItemSO.
// Holds seed-specific data for the farming system.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "SeedSO", menuName = "TheSprouty/Items/Seed")]
public class SeedSO : ItemSO
{
    [Header("Seed Info")]
    public string seedName;

    // TODO: add CropDataSO reference when farming system is ready
}
