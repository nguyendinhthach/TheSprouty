// ──────────────────────────────────────────────
// TheSprouty | Scripts/UI/WeatherIconUI.cs
// Displays the current weather icon on the HUD.
// Call SetWeather() from any system that controls weather.
// ──────────────────────────────────────────────
using System;
using UnityEngine;
using UnityEngine.UI;

public class WeatherIconUI : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [SerializeField] private Image iconImage;
    [SerializeField] private WeatherIconEntry[] iconEntries;

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Swaps the displayed icon to match the given weather type.</summary>
    public void SetWeather(WeatherType weather)
    {
        foreach (WeatherIconEntry entry in iconEntries)
        {
            if (entry.weatherType != weather) continue;
            iconImage.sprite = entry.icon;
            return;
        }

        Debug.LogWarning($"[WeatherIconUI] No icon found for WeatherType: {weather}");
    }

    // ----------------------------------------------------------
    // Nested types
    // ----------------------------------------------------------

    [Serializable]
    public class WeatherIconEntry
    {
        public WeatherType weatherType;
        public Sprite      icon;
    }
}
