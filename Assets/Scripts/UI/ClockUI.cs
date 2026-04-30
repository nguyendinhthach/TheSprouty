// ──────────────────────────────────────────────
// TheSprouty | Scripts/UI/ClockUI.cs
// Drives the analog clock hands and day counter text on the HUD.
// ──────────────────────────────────────────────
using TMPro;
using UnityEngine;

public class ClockUI : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [Header("Clock Hands")]
    [Tooltip("RectTransform of the hour hand sprite. Pivot must be at the bottom centre.")]
    [SerializeField] private RectTransform hourHand;

    [Tooltip("RectTransform of the minute hand sprite. Pivot must be at the bottom centre.")]
    [SerializeField] private RectTransform minuteHand;

    [Header("Text — 3 separate TMP objects")]
    [Tooltip("Shows the day number only, e.g. '1'")]
    [SerializeField] private TMP_Text dayNumberText;

    [Tooltip("Shows the current time, e.g. '06:00 SA'")]
    [SerializeField] private TMP_Text timeText;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------

    private int _lastMinute = -1;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Start()
    {
        if (DayCycleManager.Instance == null)
        {
            Debug.LogWarning("[ClockUI] DayCycleManager not found in scene.");
            return;
        }

        DayCycleManager.Instance.OnDayPassed += HandleDayPassed;

        UpdateDayNumber(DayCycleManager.Instance.CurrentDay);
        UpdateHands(DayCycleManager.Instance.CurrentHour);
        UpdateTimeText(DayCycleManager.Instance.CurrentHour);
    }

    private void Update()
    {
        if (DayCycleManager.Instance == null) return;

        float currentHour = DayCycleManager.Instance.CurrentHour;
        UpdateHands(currentHour);
        UpdateTimeTextIfMinuteChanged(currentHour);
    }

    private void OnDestroy()
    {
        if (DayCycleManager.Instance != null)
            DayCycleManager.Instance.OnDayPassed -= HandleDayPassed;
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    /// <summary>
    /// Rotates both clock hands based on the given hour (0.0 – 23.999).
    /// Hands must point straight up (12 o'clock) at rotation Z = 0.
    /// In Unity UI, negative Z = clockwise rotation.
    /// </summary>
    private void UpdateHands(float currentHour)
    {
        // Work in total minutes to avoid multi-step floating point error
        float totalMinutes = currentHour * 60f;

        // Minute hand: 360° per 60 min
        float minuteAngle = -(totalMinutes % 60f) / 60f * 360f;

        // Hour hand: 360° per 720 min (12h)
        float hourAngle = -(totalMinutes % 720f) / 720f * 360f;

        hourHand.localRotation   = Quaternion.Euler(0f, 0f, hourAngle);
        minuteHand.localRotation = Quaternion.Euler(0f, 0f, minuteAngle);
    }

    private void UpdateTimeTextIfMinuteChanged(float currentHour)
    {
        int currentMinute = Mathf.FloorToInt((currentHour - Mathf.Floor(currentHour)) * 60f);
        if (currentMinute == _lastMinute) return;

        _lastMinute = currentMinute;
        UpdateTimeText(currentHour);
    }

    private void UpdateTimeText(float currentHour)
    {
        if (timeText == null) return;

        int h      = Mathf.FloorToInt(currentHour);
        int m      = Mathf.FloorToInt((currentHour - h) * 60f);
        string ampm = h < 12 ? "AM" : "PM";
        int h12    = h % 12 == 0 ? 12 : h % 12;

        timeText.text = $"{h12:D2}:{m:D2} {ampm}";
    }

    private void UpdateDayNumber(int day)
    {
        if (dayNumberText != null)
            dayNumberText.text = day.ToString();
    }

    private void HandleDayPassed(object sender, int newDay)
    {
        UpdateDayNumber(newDay);
    }
}
