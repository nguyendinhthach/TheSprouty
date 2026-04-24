// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/CowDataSO.cs
// Tunable data shared by all cow variants.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "CowData", menuName = "TheSprouty/NPC/Cow Data")]
public class CowDataSO : AnimalNPCSO
{
    [Header("Player Interaction")]
    [Tooltip("Player must be within this radius to interact (Happy).")]
    public float interactRadius = 2f;

    [Header("Happy")]
    [Tooltip("Seconds before the cow can be made happy again.")]
    public float happyCooldown = 8f;

    [Header("Sleep")]
    [Tooltip("Chance per idle cycle to decide to lie down.")]
    [Range(0f, 1f)] public float sleepChance = 0.2f;
    [Tooltip("How long the cow sleeps before waking up.")]
    public float sleepDuration = 15f;
    [Tooltip("How long the cow sits idle before deciding to sleep or get up.")]
    public float sitIdleDurationMin = 2f;
    public float sitIdleDurationMax = 5f;

    [Header("Idle Sub-Actions (Sniff / Graze)")]
    [Range(0f, 1f)] public float subActionChance = 0.3f;
    public float subActionIntervalMin = 3f;
    public float subActionIntervalMax = 7f;
}
