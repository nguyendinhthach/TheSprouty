// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/FarmTileState.cs
// Enum representing the three tillable dirt states on the farm.
// ──────────────────────────────────────────────

/// <summary>
/// Progression: GrassDirt → Dirt → TilledDirt (ready for seeding).
/// Each transition requires one Hoe action.
/// </summary>
public enum FarmTileState
{
    GrassDirt,   // Default state — has grass, cannot be seeded
    Dirt,        // After 1st Hoe — grass removed, not yet tilled
    TilledDirt   // After 2nd Hoe — ready to receive seeds
}
