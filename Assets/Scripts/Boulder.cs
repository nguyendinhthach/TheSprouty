// ──────────────────────────────────────────────
// TheSprouty | Scripts/Boulder.cs
// Large rock requiring an Axe to harvest.
// Shakes on each hit via coroutine, drops stones per hit,
// and plays a 3-part destroy effect (flash + debris + scale down).
// HP scales via ResourceNodeSO.maxHealth — use different SOs for different sizes.
// ──────────────────────────────────────────────
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : Rock
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("Shake Config")]
    [Tooltip("Total duration of the shake in seconds.")]
    [SerializeField] private float shakeDuration = 0.2f;

    [Tooltip("How far the boulder offsets left/right during shake.")]
    [SerializeField] private float shakeIntensity = 0.08f;

    [Tooltip("How fast the boulder oscillates. Higher = more rapid shake.")]
    [SerializeField] private float shakeFrequency = 40f;

    [Header("Destroy Effect")]
    [Tooltip("How long the full destroy effect takes before the GameObject is removed.")]
    [SerializeField] private float destroyEffectDuration = 0.4f;

    [Tooltip("How many times the sprite flashes white before disappearing.")]
    [SerializeField] private int flashCount = 3;

    [Tooltip("Prefab spawned as visual debris (use a small pebble sprite + DebrisObject).")]
    [SerializeField] private Transform debrisPrefab;

    [Tooltip("How many debris pieces to spawn on destroy.")]
    [SerializeField] [Range(1, 6)] private int debrisCount = 3;

    [Header("Drop Per Hit")]
    [Tooltip("Items dropped on each axe hit (before the boulder is destroyed). " +
             "The final loot on destroy is handled separately by HarvestRecipeSO.dropList.")]
    [SerializeField] private List<DropEntry> dropOnHit;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Coroutine _shakeCoroutine;
    private Vector3 _originLocalPos;
    private SpriteRenderer _spriteRenderer;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    protected override void Start()
    {
        base.Start();
        _originLocalPos = transform.localPosition;
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    // ----------------------------------------------------------
    // Protected hooks
    // ----------------------------------------------------------

    /// <summary>Plays particles (via base) then starts a code-driven shake.</summary>
    protected override void OnHit(ToolSO playerTool)
    {
        base.OnHit(playerTool);
        RestartShake();
    }

    /// <summary>Starts all 3 destroy effects simultaneously: flash, debris, scale down.</summary>
    protected override void OnDestroyed()
    {
        if (_spriteRenderer != null)
            StartCoroutine(FlashRoutine());

        SpawnDebris();
        StartCoroutine(ScaleDownRoutine());
    }

    /// <summary>Matches destroyEffectDuration so ResourceNode waits for effects to finish.</summary>
    protected override float DestroyDelay => destroyEffectDuration;

    /// <summary>
    /// Drops a small amount of stone on every axe hit.
    /// The bulk drop on destroy is handled by ResourceNode.DropLoot() via HarvestRecipeSO.
    /// </summary>
    protected override void OnHitDrop()
    {
        if (dropOnHit == null || dropOnHit.Count == 0) return;

        foreach (DropEntry entry in dropOnHit)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            SpawnDrops(entry, amount);
        }
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    private void RestartShake()
    {
        if (_shakeCoroutine != null)
            StopCoroutine(_shakeCoroutine);

        _shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float offsetX = Mathf.Sin(elapsed * shakeFrequency) * shakeIntensity;
            transform.localPosition = _originLocalPos + new Vector3(offsetX, 0f, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = _originLocalPos;
        _shakeCoroutine = null;
    }

    // Blinks the sprite between white and original color flashCount times.
    private IEnumerator FlashRoutine()
    {
        Color originalColor = _spriteRenderer.color;
        float interval = destroyEffectDuration / (flashCount * 2);

        for (int i = 0; i < flashCount; i++)
        {
            _spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(interval);
            _spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(interval);
        }
    }

    // Spawns small debris pieces that arc outward using DebrisObject.
    private void SpawnDebris()
    {
        if (debrisPrefab == null) return;

        Vector3 origin = transform.position;

        for (int i = 0; i < debrisCount; i++)
        {
            Vector3 scatter = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                0f
            );

            Transform debris = Instantiate(debrisPrefab, origin, Quaternion.identity);

            if (debris.TryGetComponent<DebrisObject>(out var debrisObj))
                debrisObj.Launch(origin, origin + scatter);
        }
    }

    // Shrinks the boulder to zero scale over destroyEffectDuration.
    private IEnumerator ScaleDownRoutine()
    {
        Vector3 originalScale = transform.localScale;
        float elapsed = 0f;

        while (elapsed < destroyEffectDuration)
        {
            float t = elapsed / destroyEffectDuration;
            transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
    }
}
