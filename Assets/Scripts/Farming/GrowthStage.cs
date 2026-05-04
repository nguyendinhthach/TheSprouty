// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/GrowthStage.cs
// Enum representing each growth stage of a crop.
// Used as an index into CropDataSO.stages[].
// ──────────────────────────────────────────────

/// <summary>
/// Each value maps directly to an index in CropDataSO.stages[].
/// The number of stages is flexible per crop — defined by the SO array length.
/// </summary>
public enum GrowthStage
{
    Stage0 = 0,  // Just seeded
    Stage1 = 1,
    Stage2 = 2,
    Stage3 = 3,
    Stage4 = 4,
    Stage5 = 5,
    Stage6 = 6,
    Stage7 = 7,
}
