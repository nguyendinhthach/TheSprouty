// ──────────────────────────────────────────────
// TheSprouty | Scripts/Interfaces/IPlantable.cs
// Implemented by FarmTileManager to receive seed planting from the player.
// ──────────────────────────────────────────────

/// <summary>
/// Marks a tile system as capable of receiving seeds.
/// Called by Player.PerformToolAction when ToolType.SeedBag is equipped.
/// </summary>
public interface IPlantable
{
    /// <summary>Attempts to plant the given seed on the target cell.</summary>
    void Plant(SeedSO seed);
}
