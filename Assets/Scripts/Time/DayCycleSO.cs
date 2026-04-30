// ──────────────────────────────────────────────
// TheSprouty | Scripts/Time/DayCycleSO.cs
// ScriptableObject holding all day/night cycle configuration.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(menuName = "TheSprouty/Time/Day Cycle Config")]
public class DayCycleSO : ScriptableObject
{
    // ----------------------------------------------------------
    // Time speed
    // ----------------------------------------------------------

    [Header("Time Speed")]
    [Tooltip("How many real-world seconds equal one full 24-hour game day.")]
    public float realSecondsPerGameDay = 720f; // 12 min real = 24h game

    // ----------------------------------------------------------
    // Sleep settings
    // ----------------------------------------------------------

    [Header("Sleep Settings")]
    [Tooltip("Earliest hour the player is allowed to sleep (24h format).")]
    [Range(0, 23)] public int earliestSleepHour = 19;  // 7 PM

    [Tooltip("Hour at which the game forces the player to sleep (24h format).")]
    [Range(0, 23)] public int forcedSleepHour = 22;    // 10 PM

    [Tooltip("Hour the player wakes up at the start of each new day (24h format).")]
    [Range(0, 23)] public int wakeUpHour = 6;           // 6 AM
}
