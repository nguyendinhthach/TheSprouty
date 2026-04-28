// ──────────────────────────────────────────────
// TheSprouty | UI/RadialWheelAnimator.cs
// Reusable open/close animation for radial wheels.
// Slots fly out from center on open, collapse back on close.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;

public class RadialWheelAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private RectTransform[] slots;
    [SerializeField] private float animDuration = 0.25f;
    [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Vector2[] _targetPositions;
    private Coroutine _animRoutine;
    private CanvasGroup _canvasGroup;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _targetPositions = new Vector2[slots.Length];
        for (int i = 0; i < slots.Length; i++)
            _targetPositions[i] = slots[i].anchoredPosition;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Enable wheel then animate slots flying out.</summary>
    public void Show()
    {
        SetActiveTrue();
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(opening: true));
    }

    /// <summary>Animate slots collapsing then disable wheel.</summary>
    public void Hide()
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(opening: false, onComplete: () =>
            SetActiveFalse()));
    }

    private void SetActiveTrue()
    {
        _canvasGroup.alpha = 1;
        _canvasGroup.interactable = true;
        _canvasGroup.blocksRaycasts = true;
    }

    private void SetActiveFalse()
    {
        _canvasGroup.alpha = 0;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private IEnumerator AnimateRoutine(bool opening, Action onComplete = null)
    {
        // Snap slots to start state
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].anchoredPosition = opening ? Vector2.zero : _targetPositions[i];
            slots[i].localScale = opening ? Vector3.zero : Vector3.one;
        }

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = openCurve.Evaluate(Mathf.Clamp01(elapsed / animDuration));
            float tValue = opening ? t : 1f - t;

            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].anchoredPosition = Vector2.Lerp(Vector2.zero, _targetPositions[i], tValue);
                slots[i].localScale = Vector3.Lerp(Vector3.zero, Vector3.one, tValue);
            }

            yield return null;
        }

        // Snap to final state
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].anchoredPosition = opening ? _targetPositions[i] : Vector2.zero;
            slots[i].localScale = opening ? Vector3.one : Vector3.zero;
        }

        onComplete?.Invoke();
    }
}