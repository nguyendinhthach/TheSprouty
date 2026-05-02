// ──────────────────────────────────────────────
// TheSprouty | UI/NotificationUI.cs
// Single notification popup. Handles fade in/out animation.
// ──────────────────────────────────────────────
using System.Collections;
using TMPro;
using UnityEngine;

public class NotificationUI : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private TextMeshProUGUI message;

    [Header("Animation")]
    [SerializeField] private float fadeDuration = 0.3f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private CanvasGroup _canvasGroup;
    private Coroutine _fadeRoutine;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Show notification with given text, fade in immediately.</summary>
    public void Show(string text)
    {
        message.text = text;
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(1f));
    }

    /// <summary>Update text without restarting fade (for stacking).</summary>
    public void UpdateText(string text)
    {
        message.text = text;
    }

    /// <summary>Fade out then notify manager when done.</summary>
    public void Hide()
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeRoutine(0f));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private IEnumerator FadeRoutine(float targetAlpha)
    {
        float startAlpha = _canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            yield return null;
        }

        _canvasGroup.alpha = targetAlpha;
    }
}
