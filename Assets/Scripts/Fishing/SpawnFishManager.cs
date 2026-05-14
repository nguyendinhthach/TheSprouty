// ──────────────────────────────────────────────
// TheSprouty | Fishing/SpawnFishManager.cs
// Singleton. Manages FishShadow spawning across all FishSpawnZones.
// Relays FishShadow.OnReachedBobber → FishingController.TriggerBite().
// ──────────────────────────────────────────────
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnFishManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static SpawnFishManager Instance { get; private set; }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private FishingController fishingController;

    [Header("Settings")]
    [Tooltip("Seconds before a new shadow spawns after one is removed.")]
    [SerializeField] private float respawnDelay = 10f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private FishSpawnZone[] _zones;
    private readonly Dictionary<FishSpawnZone, List<FishShadow>> _activeShadows = new();
    private readonly Dictionary<FishShadow, FishSpawnZone> _shadowToZone = new();

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        _zones = FindObjectsByType<FishSpawnZone>(FindObjectsSortMode.None);

        foreach (FishSpawnZone zone in _zones)
        {
            _activeShadows[zone] = new List<FishShadow>();
            FillZone(zone);
        }
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Destroys shadow and schedules respawn. Called after catch or miss.</summary>
    public void RemoveShadow(FishShadow shadow)
    {
        if (!_shadowToZone.TryGetValue(shadow, out FishSpawnZone zone)) return;

        _activeShadows[zone].Remove(shadow);
        _shadowToZone.Remove(shadow);
        Destroy(shadow.gameObject);
        StartCoroutine(RespawnRoutine(zone));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void FillZone(FishSpawnZone zone)
    {
        int toSpawn = zone.MaxFishCount - _activeShadows[zone].Count;
        for (int i = 0; i < toSpawn; i++)
            SpawnOne(zone);
    }

    private void SpawnOne(FishSpawnZone zone)
    {
        GameObject prefab = zone.GetRandomShadowPrefab();
        if (prefab == null) return;

        Vector3 spawnPos  = zone.GetRandomSpawnPosition();
        GameObject go     = Instantiate(prefab, spawnPos, Quaternion.identity);
        FishShadow shadow = go.GetComponent<FishShadow>();
        if (shadow == null) return;

        _activeShadows[zone].Add(shadow);
        _shadowToZone[shadow] = zone;
        shadow.OnReachedBobber += OnShadowReachedBobber;
    }

    private void OnShadowReachedBobber(FishShadow shadow)
    {
        shadow.OnReachedBobber -= OnShadowReachedBobber;
        fishingController?.TriggerBite(shadow);
    }

    private IEnumerator RespawnRoutine(FishSpawnZone zone)
    {
        yield return new WaitForSeconds(respawnDelay);
        if (zone != null) SpawnOne(zone);
    }
}
