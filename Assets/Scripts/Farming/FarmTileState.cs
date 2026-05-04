// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/FarmTileState.cs
// Enum representing the tillable dirt states on the farm.
// ──────────────────────────────────────────────

/// <summary>
/// Progression: GrassDirt → Dirt → TilledDirt → HasCrop.
/// HasCrop: a CropObject is alive on this cell.
/// After harvest: resets back to TilledDirt.
/// </summary>
public enum FarmTileState
{
    GrassDirt,   // Default — has grass, cannot be seeded
    Dirt,        // After 1st Hoe — grass removed, not yet tilled
    TilledDirt,  // After 2nd Hoe — ready to receive seeds
    HasCrop      // CropObject is planted and alive on this cell
}
