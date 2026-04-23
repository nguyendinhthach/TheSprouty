// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenDataSO.cs
// Tunable data specific to Chicken NPCs.
// Extends AnimalNPCSO with chicken-only settings.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "ChickenData", menuName = "TheSprouty/NPC/Chicken Data")]
public class ChickenDataSO : AnimalNPCSO
{
    [Header("Player Detection")]
    [Tooltip("Chicken notices the player within this radius.")]
    public float detectRadius = 4f;
    [Tooltip("Chicken flees when player enters this radius.")]
    public float fleeRadius = 2f;

    [Header("Flee")]
    [Tooltip("Total duration of the Flee animation clip.")]
    public float fleeDuration = 1.5f;
    [Tooltip("Distance the chicken travels away from the player.")]
    public float fleeDistance = 3f;

    [Header("Nest")]
    [Tooltip("Chance per idle cycle to walk into the nest (only when nearby).")]
    [Range(0f, 1f)] public float nestGoChance = 0.1f;
    [Tooltip("Duration of sleeping inside the nest.")]
    public float nestSleepDuration = 8f;

    [Header("Sleep on Grass")]
    [Tooltip("Duration of sleeping on grass.")]
    public float sleepDuration = 5f;
    [Tooltip("Chance per idle cycle to fall asleep on grass.")]
    [Range(0f, 1f)] public float sleepChance = 0.15f;

    [Header("Happy")]
    [Tooltip("Seconds before the chicken can be made happy again.")]
    public float happyCooldown = 5f;

    [Header("Idle Sub-Actions")]
    [Tooltip("Chance to play the Eat animation as a micro-action during idle.")]
    [Range(0f, 1f)] public float eatSubActionChance = 0.25f;
    [Tooltip("Min/max seconds between eat sub-action checks.")]
    public float eatSubActionIntervalMin = 2f;
    public float eatSubActionIntervalMax = 5f;
}