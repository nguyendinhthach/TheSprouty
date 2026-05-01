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
    public float realSecondsPerGameDay = 720f;

    // ----------------------------------------------------------
    // Sleep settings
    // ----------------------------------------------------------

    [Header("Sleep Settings")]
    [Tooltip("Earliest hour the player is allowed to sleep (24h format).")]
    [Range(0, 23)] public int earliestSleepHour = 19;

    [Tooltip("Enable forced sleep when reaching forcedSleepHour. Disable for testing.")]
    public bool enableForcedSleep = true;

    [Tooltip("Hour at which the game forces the player to sleep (24h format).")]
    [Range(0, 23)] public int forcedSleepHour = 22;

    [Tooltip("Hour the player wakes up at the start of each new day (24h format).")]
    [Range(0, 23)] public int wakeUpHour = 6;

    // ----------------------------------------------------------
    // Lighting
    // ----------------------------------------------------------

    [Header("Lighting")]
    [Tooltip("Light color over 24h. Key 0 = 00:00, Key 1 = 24:00.")]
    public Gradient lightColorGradient;

    [Tooltip("Light intensity over 24h. X axis = 0..1 (00:00..24:00), Y axis = intensity.")]
    public AnimationCurve lightIntensityCurve = new AnimationCurve(
        new Keyframe(0f,      0.18f),   // 00:00 đêm
        new Keyframe(0.104f,  0.20f),   // 02:30 pre-dawn bắt đầu
        new Keyframe(0.25f,   0.55f),   // 06:00 bình minh tím
        new Keyframe(0.333f,  0.90f),   // 08:00 sáng trắng
        new Keyframe(0.5f,    0.88f),   // 12:00 trưa
        new Keyframe(0.667f,  0.90f),   // 16:00 chiều trắng
        new Keyframe(0.792f,  0.40f),   // 19:00 hoàng hôn tím
        new Keyframe(1.0f,    0.18f)    // 24:00 đêm
    );
}
