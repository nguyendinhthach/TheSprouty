// ──────────────────────────────────────────────
// TheSprouty | Items/SeedSO.cs
// ScriptableObject for seed items. Extends ItemSO.
// Links an inventory seed item to its CropDataSO.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "SeedSO", menuName = "TheSprouty/Items/Seed")]
public class SeedSO : ItemSO
{
    [Header("Seed Info")]
    public string seedName;

    [Tooltip("Crop data that this seed grows into.")]
    public CropDataSO cropData;
}
