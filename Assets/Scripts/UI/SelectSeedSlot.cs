// ──────────────────────────────────────────────
// TheSprouty | UI/SelectSeedSlot.cs
// Handles seed selection in the Seed Wheel (tier 2).
// ──────────────────────────────────────────────
using UnityEngine;

public class SelectSeedSlot : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private SeedSO seed;
    [SerializeField] private GameObject slotVisual;
    [SerializeField] private GameObject slotSelectedVisual;
    [SerializeField] private RadialSeedWheelUI radialSeedWheelUI;

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Called by Button onClick to equip this seed.</summary>
    public void SelectSeed()
    {
        Player.Instance.EquipSeed(seed);
        radialSeedWheelUI.OnSlotSelected(this);
    }

    public void SetSelected(bool selected)
    {
        slotVisual.SetActive(!selected);
        slotSelectedVisual.SetActive(selected);
    }
}
