using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private Image dimOverlay;

    [Header("Animation")]
    [SerializeField] private float animDuration = 0.4f;
    [SerializeField] private AnimationCurve slideCurve;
    [SerializeField] private float dimMaxAlpha = 0.4f;

    private Vector2 _targetPos;
    private Vector2 _hiddenPos;
    private Coroutine _animRoutine;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();

        _targetPos = panel.anchoredPosition;
        // Ẩn bên phải màn hình
        _hiddenPos = _targetPos + new Vector2(Screen.width, 0);
    }

    public void Show()
    {
        SetActiveTrue();
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(_hiddenPos, _targetPos));
    }

    public void Hide()
    {
        if (_animRoutine != null) StopCoroutine(_animRoutine);
        _animRoutine = StartCoroutine(AnimateRoutine(_targetPos, _hiddenPos,
            onComplete: () => SetActiveFalse()));
    }

    private IEnumerator AnimateRoutine(Vector2 fromPos, Vector2 toPos, Action onComplete = null)
    {
        float elapsed = 0f;
        bool opening = toPos == _targetPos;

        while (elapsed < animDuration)
        {
            elapsed += Time.deltaTime;
            float t = slideCurve.Evaluate(Mathf.Clamp01(elapsed / animDuration));
            panel.anchoredPosition = Vector2.LerpUnclamped(fromPos, toPos, t);

            // Fade DimOverlay
            float alpha = opening ? t : 1f - t;
            dimOverlay.color = new Color(0, 0, 0, alpha * dimMaxAlpha);

            yield return null;
        }
        panel.anchoredPosition = toPos;
        onComplete?.Invoke();
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
}
