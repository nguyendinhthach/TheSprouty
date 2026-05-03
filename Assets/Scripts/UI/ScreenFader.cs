// ──────────────────────────────────────────────
// TheSprouty | Scripts/UI/ScreenFader.cs
// Generic screen fade utility. Use for sleep transitions, scene changes, etc.
// Call FadeOut() / FadeIn() from any system that needs a screen fade.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------

    public static ScreenFader Instance { get; private set; }

    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------

    /// <summary>Fired when the screen has fully faded to black.</summary>
    public event EventHandler OnFadeOutComplete;

    /// <summary>Fired when the screen has fully faded back in.</summary>
    public event EventHandler OnFadeInComplete;

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [SerializeField] private Image fadeImage;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        SetAlpha(0f);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Fades screen to black over duration seconds.</summary>
    public void FadeOut(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(0f, 1f, duration, OnFadeOutComplete));
    }

    /// <summary>Fades screen back to transparent over duration seconds.</summary>
    public void FadeIn(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(FadeRoutine(1f, 0f, duration, OnFadeInComplete));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    private IEnumerator FadeRoutine(float from, float to, float duration, EventHandler onComplete)
    {
        float elapsed = 0f;
        SetAlpha(from);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, elapsed / duration));
            yield return null;
        }

        SetAlpha(to);
        onComplete?.Invoke(this, EventArgs.Empty);
    }

    private void SetAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
}
