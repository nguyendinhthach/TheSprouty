// ──────────────────────────────────────────────
// TheSprouty | Fishing/FishShadow.cs
// Controls fish shadow behaviour:
// Wander → Bobber detected → fade out → bite delay → fire OnReachedBobber.
// Shadow stays invisible until SpawnFishManager destroys it after catch/miss.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishShadow : MonoBehaviour
{
    // ----------------------------------------------------------
    // State
    // ----------------------------------------------------------
    private enum FishShadowState { Wander, Nibbling }

    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------
    /// <summary>Fired after bite delay. SpawnFishManager relays to FishingController.</summary>
    public event Action<FishShadow> OnReachedBobber;

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private SpriteRenderer visualRenderer;

    [Header("Fish Pool")]
    [Tooltip("Fish that can drop from this shadow. Rarity determines weight.")]
    [SerializeField] private FishSO[] possibleFish;

    [Header("Fade Settings")]
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private float minReactionDelay = 1f;
    [SerializeField] private float maxReactionDelay = 3f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private FishShadowState _state = FishShadowState.Wander;
    private Coroutine _activeRoutine;
    private FishSO _selectedFish;
    private CircleCollider2D _detectionCollider;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public FishSO SelectedFish => _selectedFish;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _detectionCollider = GetComponent<CircleCollider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_state != FishShadowState.Wander) return;
        if (!other.CompareTag("Bobber")) return;

        _selectedFish = GetRandomFish();
        EnterNibbling();
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Returns a random fish weighted by rarity.</summary>
    public FishSO GetRandomFish()
    {
        if (possibleFish == null || possibleFish.Length == 0) return null;

        List<FishSO> weightedPool = new();
        foreach (FishSO fish in possibleFish)
        {
            if (fish == null) continue;
            int weight = fish.rarity switch
            {
                FishRarity.Legendary => 1,
                FishRarity.Rare      => 3,
                FishRarity.Uncommon  => 6,
                FishRarity.Common    => 10,
                _                    => 10
            };
            for (int i = 0; i < weight; i++)
                weightedPool.Add(fish);
        }

        return weightedPool.Count > 0
            ? weightedPool[UnityEngine.Random.Range(0, weightedPool.Count)]
            : null;
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void EnterNibbling()
    {
        _state = FishShadowState.Nibbling;
        if (_detectionCollider != null) _detectionCollider.enabled = false;
        StopActiveRoutine();
        _activeRoutine = StartCoroutine(FadeAndBiteRoutine());
    }

    private IEnumerator FadeAndBiteRoutine()
    {
        // Chờ trước khi phản ứng với bobber
        float reactionDelay = UnityEngine.Random.Range(minReactionDelay, maxReactionDelay);
        yield return new WaitForSeconds(reactionDelay);

        yield return StartCoroutine(FadeRoutine(1f, 0f, fadeDuration));

        float biteDelay = _selectedFish != null ? _selectedFish.GetRandomBiteDelay() : 2f;
        yield return new WaitForSeconds(biteDelay);

        OnReachedBobber?.Invoke(this);
    }

    private IEnumerator FadeRoutine(float from, float to, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            SetAlpha(Mathf.Lerp(from, to, timer / duration));
            yield return null;
        }
        SetAlpha(to);
    }

    private void SetAlpha(float alpha)
    {
        if (visualRenderer == null) return;
        Color c = visualRenderer.color;
        c.a = alpha;
        visualRenderer.color = c;
    }

    private void StopActiveRoutine()
    {
        if (_activeRoutine == null) return;
        StopCoroutine(_activeRoutine);
        _activeRoutine = null;
    }
}
