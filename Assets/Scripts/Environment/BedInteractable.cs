// ──────────────────────────────────────────────
// TheSprouty | Scripts/Environment/BedInteractable.cs
// A bed the player can interact with to sleep.
// Implements IInteractable (indicator feedback) and IUsable (actual use).
// ──────────────────────────────────────────────
using System;
using UnityEngine;

public class BedInteractable : MonoBehaviour, IInteractable, IUsable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [Tooltip("Position the player is moved to after waking up (set as child Transform).")]
    [SerializeField] private Transform wakeUpPoint;

    [Tooltip("Visual feedback when indicator is hovering (e.g. highlight sprite).")]
    [SerializeField] private GameObject highlightObject;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Start()
    {
        if (DayCycleManager.Instance != null)
            DayCycleManager.Instance.OnSleepEnded += HandleSleepEnded;

        SetHighlight(false);
    }

    private void OnDestroy()
    {
        if (DayCycleManager.Instance != null)
            DayCycleManager.Instance.OnSleepEnded -= HandleSleepEnded;
    }

    // ----------------------------------------------------------
    // IInteractable
    // ----------------------------------------------------------

    public void OnIndicatorEnter() => SetHighlight(true);
    public void OnIndicatorExit()  => SetHighlight(false);

    // ----------------------------------------------------------
    // IUsable
    // ----------------------------------------------------------

    /// <summary>Called by Player when ToolType.None and this bed is targeted.</summary>
    public void Use()
    {
        bool sleepStarted = DayCycleManager.Instance.TrySleep();

        if (!sleepStarted)
        {
            NotificationManager.Instance?.ShowMessage("Not time to sleep yet!");
        }
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    private void HandleSleepEnded(object sender, EventArgs e)
    {
        if (wakeUpPoint == null || Player.Instance == null) return;
        Player.Instance.transform.position = wakeUpPoint.position;
    }

    private void SetHighlight(bool active)
    {
        if (highlightObject != null)
            highlightObject.SetActive(active);
    }
}
