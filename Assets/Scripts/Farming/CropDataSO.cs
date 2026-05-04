// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/CropDataSO.cs
// ScriptableObject holding all data for one crop type.
// One SO per crop (e.g. Turnip, Carrot, Tomato).
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "New Crop", menuName = "TheSprouty/Farming/Crop Data")]
public class CropDataSO : ScriptableObject
{
    // ----------------------------------------------------------
    // Identity
    // ----------------------------------------------------------

    [Header("Identity")]
    public string cropName;

    // ----------------------------------------------------------
    // Growth stages
    // ----------------------------------------------------------

    [Header("Growth Stages")]
    [Tooltip("One entry per stage. Index 0 = just seeded, last index = Mature. " +
             "hoursToNextStage of the last entry is ignored.")]
    public CropStageData[] stages;

    [Tooltip("How many times slower the crop grows when NOT watered. " +
             "e.g. 2 = takes twice as long without water.")]
    [Min(1f)]
    public float unwateredSlowdownMultiplier = 2f;

    // ----------------------------------------------------------
    // Harvest
    // ----------------------------------------------------------

    [Header("Harvest")]
    [Tooltip("Items dropped when the crop is harvested.")]
    public DropEntry[] yield;

    // ----------------------------------------------------------
    // Helpers
    // ----------------------------------------------------------

    /// <summary>Returns true if the given stage index is the last (Mature) stage.</summary>
    public bool IsMature(int stageIndex) => stageIndex >= stages.Length - 1;

    /// <summary>Returns the hours needed to advance from the given stage, accounting for watering.</summary>
    public float GetHoursToAdvance(int stageIndex, bool isWatered)
    {
        if (stageIndex >= stages.Length - 1) return float.MaxValue; // already mature

        float baseHours = stages[stageIndex].hoursToNextStage;
        return isWatered ? baseHours : baseHours * unwateredSlowdownMultiplier;
    }
}
