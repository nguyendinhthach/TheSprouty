// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/AnimalNPCSO.cs
// Shared tunable data for all animal NPCs.
// Subclass this for animal-specific settings.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalNPCData", menuName = "TheSprouty/NPC/Animal NPC Data")]
public class AnimalNPCSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float wanderRadiusMin = 1f;
    public float wanderRadiusMax = 4f;

    [Header("Idle")]
    public float idleTimeMin = 2f;
    public float idleTimeMax = 5f;
}
