// ──────────────────────────────────────────────
// TheSprouty | Scripts/Rock.cs
// Abstract base for all rock-type ResourceNodes.
// Plays hit particles on each hit. Subclasses add their own behaviour.
// ──────────────────────────────────────────────
using UnityEngine;

public abstract class Rock : ResourceNode
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    //[Header("Rock FX")]
    //[SerializeField] private ParticleSystem hitParticles;

    // ----------------------------------------------------------
    // Protected hooks
    // ----------------------------------------------------------

    /// <summary>Plays hit particles. Subclasses call base.OnHit() to keep this behaviour.</summary>
    protected override void OnHit(ToolSO playerTool)
    {
        //hitParticles?.Play();
    }
}
