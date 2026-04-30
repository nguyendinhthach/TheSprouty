// ──────────────────────────────────────────────
// TheSprouty | Scripts/Time/DayCycleManager.cs
// Singleton that tracks game time, raises time events, and manages sleep.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;

public class DayCycleManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------

    /// <summary>Fired every time the integer hour changes (0–23).</summary>
    public event EventHandler<int> OnHourChanged;

    /// <summary>Fired at the start of each new day. Arg = new day number.</summary>
    public event EventHandler<int> OnDayPassed;

    /// <summary>Fired when the player begins sleeping (use for fade-out).</summary>
    public event EventHandler OnSleepStarted;

    /// <summary>Fired when the player wakes up (use for fade-in).</summary>
    public event EventHandler OnSleepEnded;

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [SerializeField] private DayCycleSO dayCycleConfig;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------

    private float _currentHour;
    private int   _currentDay  = 1;
    private int   _lastHour    = -1;
    private bool  _isSleeping  = false;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------

    public static DayCycleManager Instance { get; private set; }

    /// <summary>Current game hour as a float (0.0 – 23.999).</summary>
    public float CurrentHour => _currentHour;

    /// <summary>Current game day, starting at 1.</summary>
    public int CurrentDay => _currentDay;

    /// <summary>Current period of the day derived from CurrentHour.</summary>
    public TimeOfDay CurrentTimeOfDay => EvaluateTimeOfDay(_currentHour);

    public bool IsSleeping => _isSleeping;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        _currentHour = dayCycleConfig.wakeUpHour;
        _lastHour    = dayCycleConfig.wakeUpHour;
    }

    private void Update()
    {
        if (_isSleeping) return;

        AdvanceTime();
        CheckHourChanged();
        CheckForcedSleep();
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>
    /// Attempts to put the player to sleep.
    /// Returns false (and does nothing) if it is before earliestSleepHour.
    /// </summary>
    public bool TrySleep()
    {
        if (Mathf.FloorToInt(_currentHour) < dayCycleConfig.earliestSleepHour)
            return false;

        StartSleep();
        return true;
    }

    /// <summary>Returns a formatted string "HH:MM" for the current game time.</summary>
    public string GetFormattedTime()
    {
        int h = Mathf.FloorToInt(_currentHour);
        int m = Mathf.FloorToInt((_currentHour - h) * 60f);
        return $"{h:D2}:{m:D2}";
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    private void AdvanceTime()
    {
        float hoursPerSecond = 24f / dayCycleConfig.realSecondsPerGameDay;
        _currentHour += Time.deltaTime * hoursPerSecond;

        if (_currentHour >= 24f)
            _currentHour -= 24f;
    }

    private void CheckHourChanged()
    {
        int hour = Mathf.FloorToInt(_currentHour);
        if (hour == _lastHour) return;

        _lastHour = hour;
        OnHourChanged?.Invoke(this, hour);
    }

    private void CheckForcedSleep()
    {
        if (Mathf.FloorToInt(_currentHour) >= dayCycleConfig.forcedSleepHour)
            StartSleep();
    }

    private void StartSleep()
    {
        if (_isSleeping) return;

        _isSleeping = true;
        OnSleepStarted?.Invoke(this, EventArgs.Empty);
        StartCoroutine(SleepRoutine());
    }

    private IEnumerator SleepRoutine()
    {
        // Pause here — ClockUI / fade systems listen to OnSleepStarted
        yield return new WaitForSeconds(1f);

        _currentDay++;
        _currentHour = dayCycleConfig.wakeUpHour;
        _lastHour    = dayCycleConfig.wakeUpHour;

        OnDayPassed?.Invoke(this, _currentDay);

        yield return new WaitForSeconds(0.5f);

        _isSleeping = false;
        OnSleepEnded?.Invoke(this, EventArgs.Empty);
    }

    private TimeOfDay EvaluateTimeOfDay(float hour)
    {
        if (hour >= 5f  && hour < 7f)  return TimeOfDay.Dawn;
        if (hour >= 7f  && hour < 11f) return TimeOfDay.Morning;
        if (hour >= 11f && hour < 14f) return TimeOfDay.Noon;
        if (hour >= 14f && hour < 17f) return TimeOfDay.Afternoon;
        if (hour >= 17f && hour < 20f) return TimeOfDay.Dusk;
        return TimeOfDay.Night;
    }
}
