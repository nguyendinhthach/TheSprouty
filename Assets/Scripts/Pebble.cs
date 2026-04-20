// ──────────────────────────────────────────────
// TheSprouty | Scripts/Pebble.cs
// Small rock that any tool (including bare hands) can interact with.
// Single hit → drops 1 stone. No shake animation.
// Configure via HarvestRecipeSO: leave validTools[] empty → any tool valid.
// Configure via ResourceNodeSO: maxHealth = 1 → destroyed in one hit.
// ──────────────────────────────────────────────
using UnityEngine;

public class Pebble : Rock
{
    // No additional logic needed.
    // All behaviour is inherited from Rock (particles) and ResourceNode (damage, drop).
    // Data-driven entirely through HarvestRecipeSO + ResourceNodeSO.
}
