// ──────────────────────────────────────────────
// TheSprouty | UI/DialogueAnimator.cs
// Slide-up + fade animation for the DialoguePanel.
// Attach on the same GameObject as DialoguePanelUI.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;

public class DialogueAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private RectTransform panel;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.3f;
    [SerializeField] private float slideOffset  = 80f;
    [SerializeField] private AnimationCurve slideCurve;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Vector2 _targetPos;
    private Vector2 _hiddenPos;
    private Coroutine _animRoutine;
    private CanvasGroup _canvasGroup;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _targetPos = panel.anchoredPosition;
        _hiddenPos = _targetPos - new Vector2(0f, slideOffset);

        SetVisible(false);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------
    /// <summary>Plays show animation. onComplete fires when animation finishes.</summary>
    public void Show(Action onComplete = null)
    {
        SetVisible(true);
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(_hiddenPos, _targetPos, 0f, 1f, onComplete));
    }

    /// <summary>Plays hide animation then deactivates the panel.</summary>
    public void Hide(Action onComplete = null)
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(_targetPos, _hiddenPos, 1f, 0f,
            onComplete: () =>
            {
                SetVisible(false);
                onComplete?.Invoke();
            }));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private IEnumerator AnimateRoutine(Vector2 fromPos, Vector2 toPos,
                                       float fromAlpha, float toAlpha,
                                       Action onComplete = null)
    {
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / animDuration));

            panel.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, t);
            _canvasGroup.alpha      = Mathf.Lerp(fromAlpha, toAlpha, t);

            yield return null;
        }

        panel.anchoredPosition = toPos;
        _canvasGroup.alpha      = toAlpha;
        _animRoutine = null;
        onComplete?.Invoke();
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha          = visible ? 1f : 0f;
        _canvasGroup.interactable   = visible;
        _canvasGroup.blocksRaycasts = visible;
    }
}
