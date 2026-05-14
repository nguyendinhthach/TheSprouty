// ──────────────────────────────────────────────
// TheSprouty | Fishing/FishingController.cs
// Manages the core fishing state machine.
// ──────────────────────────────────────────────
// TODO: replace Input.GetMouseButtonDown(1) with InputSystem when stable.
// ──────────────────────────────────────────────
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishingController : MonoBehaviour
{
    // ----------------------------------------------------------
    // State
    // ----------------------------------------------------------
    private enum FishingState { Inactive, Casting, WaitingForBite, NibbleWindow, Reeling }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private PlayerAnimator playerAnimator;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;
    [SerializeField] private Tilemap waterTilemap;

    [Header("Bobber")]
    [SerializeField] private GameObject bobberPrefab;

    [Header("Timing (seconds)")]
    [SerializeField] private float nibbleWindowDuration = 1.5f;
    [SerializeField] private float minReelDuration = 1f;
    [SerializeField] private float maxReelDuration = 3f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Player _player;
    private FishingState _state = FishingState.Inactive;
    private Coroutine _activeRoutine;
    private bool _isOnWater;
    private BobberAnimator _activeBobber;
    private GameObject _activeBobberGO;
    private FishSO _caughtFish;
    private FishShadow _activeShadow;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public bool IsFishing => _state != FishingState.Inactive;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _player = GetComponent<Player>();
    }

    private void Start()
    {
        gameInput.OnUseToolAction += OnLMBPressed;
    }

    private void OnDestroy()
    {
        gameInput.OnUseToolAction -= OnLMBPressed;
    }

    private void Update()
    {
        if ((_state == FishingState.WaitingForBite || _state == FishingState.NibbleWindow)
            && Input.GetMouseButtonDown(1))
        {
            StopActiveRoutine();
            ExitFishing();
        }
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Called by PlayerAnimator when FishingRod tool is used — cast animation starts.</summary>
    public void OnCastStart()
    {
        _isOnWater = IsIndicatorOnWater();
        playerAnimator.SetIsFishingOnWater(_isOnWater);
        _state = FishingState.Casting;
    }

    /// <summary>Called by AnimationEvent_CastComplete when cast animation finishes.</summary>
    public void OnCastComplete()
    {
        if (_state != FishingState.Casting) return;

        if (!_isOnWater)
        {
            _state = FishingState.Inactive;
            _player.UnlockAction();
            return;
        }

        _state = FishingState.WaitingForBite;
        SpawnBobber();
    }

    /// <summary>Called by AnimationEvent_FishingComplete on last frame of Fishing_Happy.</summary>
    public void OnFishingComplete()
    {
        _state = FishingState.Inactive;
        playerIndicator.gameObject.SetActive(true);
        _player.UnlockAction();
    }

    /// <summary>Called by SpawnFishManager when a shadow reaches the bobber.</summary>
    public void TriggerBite(FishShadow shadow)
    {
        if (_state != FishingState.WaitingForBite) return;

        _activeShadow  = shadow;
        _caughtFish    = shadow.SelectedFish;
        _state         = FishingState.NibbleWindow;
        _activeBobber?.PlayBitten();
        _activeRoutine = StartCoroutine(NibbleWindowRoutine());
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void OnLMBPressed(object sender, EventArgs e)
    {
        if (_state != FishingState.NibbleWindow) return;

        StopActiveRoutine();
        CatchFish();
    }

    private IEnumerator NibbleWindowRoutine()
    {
        yield return new WaitForSeconds(nibbleWindowDuration);

        _activeBobber?.ResetToIdle();
        _state = FishingState.WaitingForBite;
        SpawnFishManager.Instance?.RemoveShadow(_activeShadow);
        _activeShadow = null;
    }

    private void CatchFish()
    {
        _state = FishingState.Reeling;
        DespawnBobber();
        playerIndicator.gameObject.SetActive(false);
        playerAnimator.TriggerFishingReel();
        _activeRoutine = StartCoroutine(ReelLoopRoutine());
    }

    private IEnumerator ReelLoopRoutine()
    {
        float duration = UnityEngine.Random.Range(minReelDuration, maxReelDuration);
        yield return new WaitForSeconds(duration);

        playerAnimator.TriggerFishingCatch();

        if (_caughtFish != null && InventoryManager.Instance != null)
            InventoryManager.Instance.AddItem(_caughtFish, 1);

        SpawnFishManager.Instance?.RemoveShadow(_activeShadow);
        _activeShadow = null;
        _caughtFish   = null;
    }

    private void ExitFishing()
    {
        DespawnBobber();
        playerAnimator.TriggerFishingExit();
        _player.UnlockAction();
        StartCoroutine(SetInactiveNextFrameRoutine());
    }

    private void SpawnBobber()
    {
        if (bobberPrefab == null) return;
        _activeBobberGO = Instantiate(bobberPrefab, playerIndicator.transform.position, Quaternion.identity);
        _activeBobber   = _activeBobberGO.GetComponentInChildren<BobberAnimator>();
    }

    private void DespawnBobber()
    {
        if (_activeBobberGO == null) return;
        Destroy(_activeBobberGO);
        _activeBobberGO = null;
        _activeBobber   = null;
    }

    private IEnumerator SetInactiveNextFrameRoutine()
    {
        yield return null;
        _state = FishingState.Inactive;
    }

    private void StopActiveRoutine()
    {
        if (_activeRoutine == null) return;
        StopCoroutine(_activeRoutine);
        _activeRoutine = null;
    }

    private bool IsIndicatorOnWater()
    {
        if (waterTilemap == null || playerIndicator == null) return false;
        Vector3Int cell = waterTilemap.WorldToCell(playerIndicator.transform.position);
        return waterTilemap.GetTile(cell) != null;
    }
}
