// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/CropStageData.cs
// Data for one growth stage of a crop.
// Stored as an array in CropDataSO.
// ──────────────────────────────────────────────
using UnityEngine;

[System.Serializable]
public class CropStageData
{
    [Tooltip("Sprite displayed when the crop is at this stage.")]
    public Sprite sprite;

    [Tooltip("In-game hours needed to advance to the next stage. " +
             "Ignored for the final (Mature) stage.")]
    public float hoursToNextStage;
}
