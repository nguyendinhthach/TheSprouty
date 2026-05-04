// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/CropObject.cs
// Represents a planted crop on a farm tile.
// Handles growth over in-game hours, watering check,
// sprite swapping, and harvest via SpawnDrops.
// ──────────────────────────────────────────────
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class CropObject : MonoBehaviour, IInteractable, IDroppable, IUsable, IWaterable
{
    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------

    private CropDataSO      _data;
    private FarmTileManager _tileManager;
    private Vector3Int      _cell;
    private SpriteRenderer  _spriteRenderer;
    private int             _currentStageIndex;
    private float           _hoursAccumulated;
    private bool            _isMature;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        DayCycleManager.Instance.OnHourChanged += OnHourChanged;
    }

    private void OnDestroy()
    {
        if (DayCycleManager.Instance != null)
            DayCycleManager.Instance.OnHourChanged -= OnHourChanged;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>
    /// Initialises the crop. Called by FarmTileManager after spawning.
    /// </summary>
    public void Initialise(CropDataSO data, FarmTileManager tileManager, Vector3Int cell)
    {
        _data             = data;
        _tileManager      = tileManager;
        _cell             = cell;
        _currentStageIndex = 0;
        _hoursAccumulated  = 0f;
        _isMature          = false;

        UpdateSprite();
    }

    // ----------------------------------------------------------
    // IInteractable — PlayerIndicator hover feedback
    // ----------------------------------------------------------

    public void OnIndicatorEnter() { }
    public void OnIndicatorExit()  { }

    // ----------------------------------------------------------
    // IDroppable — harvest loot
    // ----------------------------------------------------------

    /// <summary>Spawns yield drops using ItemBounceObject arcs.</summary>
    public void DropLoot()
    {
        if (_data?.yield == null) return;

        foreach (DropEntry entry in _data.yield)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            SpawnDrops(entry, amount);
        }
    }

    // ----------------------------------------------------------
    // Private — growth logic
    // ----------------------------------------------------------

    private void OnHourChanged(object sender, int hour)
    {
        if (_isMature) return;

        bool  isWatered   = _tileManager.IsWatered(_cell);
        float hoursGained = isWatered ? 1f : 1f / _data.unwateredSlowdownMultiplier;

        _hoursAccumulated += hoursGained;

        float required = _data.stages[_currentStageIndex].hoursToNextStage;

        if (_hoursAccumulated >= required)
        {
            _hoursAccumulated -= required;
            AdvanceStage();
        }
    }

    private void AdvanceStage()
    {
        _currentStageIndex++;

        if (_data.IsMature(_currentStageIndex))
        {
            _currentStageIndex = _data.stages.Length - 1;
            _isMature = true;
        }

        UpdateSprite();
    }

    private void UpdateSprite()
    {
        if (_data == null || _currentStageIndex >= _data.stages.Length) return;
        _spriteRenderer.sprite = _data.stages[_currentStageIndex].sprite;
    }

    // ----------------------------------------------------------
    // IUsable — harvest bằng tay không (ToolType.None)
    // ----------------------------------------------------------

    /// <summary>
    /// Called by Player.PerformToolAction when ToolType.None.
    /// Only harvests if the crop is Mature.
    /// </summary>
    public void Use() => Harvest();

    // ----------------------------------------------------------
    // IWaterable — WateringCan khi crop đang trên tile
    // ----------------------------------------------------------

    /// <summary>
    /// Delegates watering to FarmTileManager for this crop's cell.
    /// Allows WateringCan to work even when CropObject is the detected target.
    /// </summary>
    public void Water(ToolSO tool) => _tileManager.SetWatered(_cell);

    // ----------------------------------------------------------
    // Private — harvest
    // ----------------------------------------------------------

    /// <summary>
    /// Drops loot, resets the tile to TilledDirt, then destroys this object.
    /// </summary>
    private void Harvest()
    {
        if (!_isMature) return;

        Player.Instance.ClearCurrentTarget();
        DropLoot();
        _tileManager.SetState(_cell, FarmTileState.Dirt);
        Destroy(gameObject);
    }

    private void SpawnDrops(DropEntry entry, int amount)
    {
        if (entry.item?.prefab == null) return;

        Vector3 origin = transform.position;
        for (int i = 0; i < amount; i++)
        {
            Vector3 scatter = new Vector3(
                Random.Range(-0.8f, 0.8f),
                Random.Range(-0.8f, 0.8f),
                0f
            );
            Transform spawned = Instantiate(entry.item.prefab, origin, Quaternion.identity);
            if (spawned.TryGetComponent<ItemBounceObject>(out var bounce))
                bounce.StartBounce(origin, origin + scatter);
        }
    }
}
