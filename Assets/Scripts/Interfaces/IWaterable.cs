// ──────────────────────────────────────────────
// TheSprouty | Scripts/Interfaces/IWaterable.cs
// Implemented by objects that can be watered by the WateringCan tool.
// FarmTileManager implements this to handle watering on Dirt and TilledDirt cells.
// ──────────────────────────────────────────────

/// <summary>
/// Marks an object as waterable by the player's WateringCan tool.
/// </summary>
public interface IWaterable
{
    /// <summary>Called by Player.PerformToolAction when ToolType.WateringCan is equipped.</summary>
    void Water(ToolSO tool);
}
