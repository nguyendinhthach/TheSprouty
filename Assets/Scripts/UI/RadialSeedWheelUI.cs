// ──────────────────────────────────────────────
// TheSprouty | UI/RadialSeedWheelUI.cs
// Manages the Seed Wheel (tier 2). Opened by SeedBagSlot,
// closed by TAB toggle.
// ──────────────────────────────────────────────
using System;
using UnityEngine;

public class RadialSeedWheelUI : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private SelectSeedSlot[] seedSlots;
    [SerializeField] private RadialWheelAnimator wheelAnimator;
    [SerializeField] private PlayerIndicator playerIndicator;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private CanvasGroup _canvasGroup;
    private bool _isVisible = false;

    public static bool IsOpen { get; private set; }

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetVisible(false);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    public void Open()
    {
        _isVisible = true;
        IsOpen = true;
        wheelAnimator.Show();
        playerIndicator.gameObject.SetActive(false);
    }

    public void Close()
    {
        _isVisible = false;
        IsOpen = false;
        wheelAnimator.Hide();
        playerIndicator.gameObject.SetActive(true);
    }

    public void OnSlotSelected(SelectSeedSlot selectedSlot)
    {
        foreach (SelectSeedSlot slot in seedSlots)
            slot.SetSelected(slot == selectedSlot);
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1 : 0;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }
}
