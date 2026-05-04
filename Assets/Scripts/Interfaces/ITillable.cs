// ──────────────────────────────────────────────
// TheSprouty | Scripts/Interfaces/ITillable.cs
// Implemented by objects that can be tilled by the Hoe tool.
// ──────────────────────────────────────────────

/// <summary>
/// Marks an object as tillable by the player's Hoe tool.
/// FarmTileManager implements this to advance dirt state per cell.
/// </summary>
public interface ITillable
{
    /// <summary>Called by Player.PerformToolAction when ToolType.Hoe is equipped.</summary>
    void Till(ToolSO tool);
}
