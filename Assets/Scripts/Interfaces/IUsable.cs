// ──────────────────────────────────────────────
// TheSprouty | Scripts/Interfaces/IUsable.cs
// Implemented by objects the player can "use" when no tool is equipped
// (e.g. bed, chest, NPC, shop counter).
// ──────────────────────────────────────────────

/// <summary>
/// Marks an object as usable by the player when ToolType.None is equipped.
/// Distinct from IInteractable (indicator hover feedback) — this drives actual interaction.
/// </summary>
public interface IUsable
{
    /// <summary>Called by Player.PerformToolAction when ToolType.None.</summary>
    void Use();
}
