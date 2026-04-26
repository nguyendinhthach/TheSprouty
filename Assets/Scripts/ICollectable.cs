// ──────────────────────────────────────────────
// TheSprouty | Scripts/ICollectable.cs
// Contract for any object that can be collected by the player.
// ──────────────────────────────────────────────

public interface ICollectable
{
    string ObjectName { get; }

    /// <summary>
    /// Called by ItemPickup when the player collects this object.
    /// </summary>
    void OnCollected();
}
