// ──────────────────────────────────────────────
// TheSprouty | Scripts/Farming/FarmTileManager.cs
// Manages the farm Tilemap: tracks per-cell dirt state, swaps tile visuals,
// spawns tilled mark prefabs, handles watering tint, and reverts untilled
// Dirt back to GrassDirt based on absolute in-game hours.
// ──────────────────────────────────────────────
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
[RequireComponent(typeof(TilemapCollider2D))]
public class FarmTileManager : MonoBehaviour, IInteractable, ITillable, IWaterable, IPlantable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [Header("References")]
    [SerializeField] private PlayerIndicator playerIndicator;

    [Header("Tile Visuals")]
    [Tooltip("Default tile with grass — starting state.")]
    [SerializeField] private TileBase grassDirtTile;

    [Tooltip("Plain dirt tile after 1st Hoe hit — grass removed.")]
    [SerializeField] private TileBase dirtTile;

    [Header("Tilled Mark")]
    [Tooltip("Prefab spawned on top of the dirt tile after 2nd Hoe hit (plow marks visual).")]
    [SerializeField] private GameObject tilledMarkPrefab;

    [Header("Crop")]
    [Tooltip("Base prefab for all CropObjects. Must have CropObject component + SpriteRenderer.")]
    [SerializeField] private GameObject cropObjectPrefab;

    [Header("Watering")]
    [Tooltip("Tint applied to a cell when watered. Pure dark grey = just darker, no hue shift.")]
    [SerializeField] private Color wateredTint = new Color(0.6f, 0.6f, 0.6f, 1f);

    [Header("Settings")]
    [Tooltip("In-game hours before an untilled Dirt cell reverts back to GrassDirt.")]
    [SerializeField] private int dirtRevertHours = 24;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------

    private Tilemap _tilemap;

    /// <summary>Tracks cells whose state differs from GrassDirt (default).</summary>
    private readonly Dictionary<Vector3Int, FarmTileState> _tileStates = new();

    /// <summary>Spawned tilled-mark GameObjects keyed by cell position.</summary>
    private readonly Dictionary<Vector3Int, GameObject> _tilledMarks = new();

    /// <summary>Cells that have been watered this day.</summary>
    private readonly HashSet<Vector3Int> _wateredCells = new();

    /// <summary>
    /// Absolute in-game hour when each Dirt cell was created.
    /// Elapsed = currentTotalHour - createdAtHour.
    /// </summary>
    private readonly Dictionary<Vector3Int, int> _dirtCreatedAtHour = new();

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Awake()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    private void Start()
    {
        DayCycleManager.Instance.OnHourChanged += OnHourChanged;
        DayCycleManager.Instance.OnDayPassed   += OnDayPassed;
    }

    private void OnDestroy()
    {
        if (DayCycleManager.Instance == null) return;
        DayCycleManager.Instance.OnHourChanged -= OnHourChanged;
        DayCycleManager.Instance.OnDayPassed   -= OnDayPassed;
    }

    // ----------------------------------------------------------
    // IInteractable
    // ----------------------------------------------------------

    public void OnIndicatorEnter() { }
    public void OnIndicatorExit()  { }

    // ----------------------------------------------------------
    // ITillable — Hoe tool
    // ----------------------------------------------------------

    /// <summary>
    /// Advances the dirt state one step: GrassDirt → Dirt → TilledDirt.
    /// Does nothing on TilledDirt.
    /// </summary>
    public void Till(ToolSO tool)
    {
        Vector3Int cell = GetIndicatorCell();
        if (_tilemap.GetTile(cell) == null) return;

        switch (GetState(cell))
        {
            case FarmTileState.GrassDirt: SetState(cell, FarmTileState.Dirt);       break;
            case FarmTileState.Dirt:      SetState(cell, FarmTileState.TilledDirt); break;
            case FarmTileState.HasCrop:   return; // cannot hoe a tile with a crop
        }
    }

    // ----------------------------------------------------------
    // IWaterable — WateringCan tool
    // ----------------------------------------------------------

    /// <summary>
    /// Waters the cell under the PlayerIndicator if it is Dirt or TilledDirt.
    /// GrassDirt cannot be watered. Already-watered cells are ignored.
    /// </summary>
    public void Water(ToolSO tool) => ApplyWater(GetIndicatorCell());

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Returns the current FarmTileState of a given cell.</summary>
    public FarmTileState GetState(Vector3Int cell)
    {
        return _tileStates.TryGetValue(cell, out FarmTileState state)
            ? state
            : FarmTileState.GrassDirt;
    }

    /// <summary>Returns true if the cell has been watered today.</summary>
    public bool IsWatered(Vector3Int cell) => _wateredCells.Contains(cell);

    // ----------------------------------------------------------
    // IPlantable — SeedBag tool
    // ----------------------------------------------------------

    /// <summary>
    /// Plants a seed on the cell under the PlayerIndicator.
    /// Requires TilledDirt state, seed in inventory, and valid CropData.
    /// </summary>
    public void Plant(SeedSO seed)
    {
        if (seed == null || seed.cropData == null)
        {
            Debug.LogWarning("[FarmTile] Plant() called with null seed or missing CropData.");
            return;
        }

        Vector3Int cell = GetIndicatorCell();
        FarmTileState cellState = GetState(cell);

        if (cellState != FarmTileState.TilledDirt) return;

        // TODO: uncomment when inventory is wired up properly
        // if (!InventoryManager.Instance.HasItem(seed))
        // {
        //     Debug.Log("[FarmTile] Plant() ignored — no seed in inventory.");
        //     return;
        // }
        // InventoryManager.Instance.RemoveItem(seed, 1);

        // Spawn CropObject at cell center
        Vector3 worldCenter = _tilemap.GetCellCenterWorld(cell);
        GameObject cropGO   = Instantiate(cropObjectPrefab, worldCenter, Quaternion.identity);
        CropObject crop     = cropGO.GetComponent<CropObject>();
        crop.Initialise(seed.cropData, this, cell);

        SetState(cell, FarmTileState.HasCrop);

    }

    /// <summary>
    /// Directly waters a specific cell. Called by CropObject.Water()
    /// when the crop is the detected IInteractable target.
    /// </summary>
    public void SetWatered(Vector3Int cell) => ApplyWater(cell);

    /// <summary>
    /// Sets a cell to a given state, updates tile visuals and manages tilled marks.
    /// Called externally by CropObject to reset a cell after harvest.
    /// </summary>
    public void SetState(Vector3Int cell, FarmTileState newState)
    {
        switch (newState)
        {
            case FarmTileState.GrassDirt:
                _tilemap.SetTile(cell, grassDirtTile);
                _tilemap.SetColor(cell, Color.white);
                RemoveTilledMark(cell);
                _tileStates.Remove(cell);
                _dirtCreatedAtHour.Remove(cell);
                _wateredCells.Remove(cell);
                break;

            case FarmTileState.Dirt:
                _tilemap.SetTile(cell, dirtTile);
                // Preserve watered tint if cell was already watered
                _tilemap.SetColor(cell, _wateredCells.Contains(cell) ? wateredTint : Color.white);
                RemoveTilledMark(cell);
                _tileStates[cell]        = FarmTileState.Dirt;
                _dirtCreatedAtHour[cell] = GetTotalGameHour();
                break;

            case FarmTileState.TilledDirt:
                _tilemap.SetColor(cell, _wateredCells.Contains(cell) ? wateredTint : Color.white);
                _tileStates[cell] = FarmTileState.TilledDirt;
                _dirtCreatedAtHour.Remove(cell);
                SpawnTilledMark(cell);
                break;

            case FarmTileState.HasCrop:
                _tileStates[cell] = FarmTileState.HasCrop;
                RemoveTilledMark(cell); // crop object is now the visual
                break;
        }
    }

    /// <summary>Converts a world position to the cell coordinate on this tilemap.</summary>
    public Vector3Int WorldToCell(Vector3 worldPosition)
    {
        return _tilemap.WorldToCell(worldPosition);
    }

    // ----------------------------------------------------------
    // Private event handlers
    // ----------------------------------------------------------

    private void OnHourChanged(object sender, int hour)
    {
        if (_dirtCreatedAtHour.Count == 0) return;

        int              currentTotal = GetTotalGameHour();
        List<Vector3Int> cells        = new(_dirtCreatedAtHour.Keys);
        List<Vector3Int> toRevert     = new();

        foreach (Vector3Int cell in cells)
        {
            int elapsed = currentTotal - _dirtCreatedAtHour[cell];

            if (elapsed >= dirtRevertHours)
                toRevert.Add(cell);
        }

        foreach (Vector3Int cell in toRevert)
            SetState(cell, FarmTileState.GrassDirt);
    }

    private void OnDayPassed(object sender, int day)
    {
        foreach (Vector3Int cell in _wateredCells)
        {
            _tilemap.SetColor(cell, Color.white);
            ApplyTintToMark(cell, Color.white);
        }
        _wateredCells.Clear();
    }

    // ----------------------------------------------------------
    // Private helpers
    // ----------------------------------------------------------

    /// <summary>Total in-game hours since day 1. Always increases.</summary>
    private int GetTotalGameHour()
    {
        return (DayCycleManager.Instance.CurrentDay - 1) * 24
               + Mathf.FloorToInt(DayCycleManager.Instance.CurrentHour);
    }

    private Vector3Int GetIndicatorCell()
    {
        return _tilemap.WorldToCell(playerIndicator.transform.position);
    }

    private void ApplyWater(Vector3Int cell)
    {
        if (GetState(cell) == FarmTileState.GrassDirt) return;
        if (_wateredCells.Contains(cell))              return;

        _wateredCells.Add(cell);
        _tilemap.SetColor(cell, wateredTint);
        ApplyTintToMark(cell, wateredTint);
    }

    private void SpawnTilledMark(Vector3Int cell)
    {
        if (tilledMarkPrefab == null)       return;
        if (_tilledMarks.ContainsKey(cell)) return;

        Vector3 worldCenter = _tilemap.GetCellCenterWorld(cell);
        GameObject mark = Instantiate(tilledMarkPrefab, worldCenter, Quaternion.identity, transform);
        _tilledMarks[cell] = mark;
    }

    private void RemoveTilledMark(Vector3Int cell)
    {
        if (!_tilledMarks.TryGetValue(cell, out GameObject mark)) return;
        if (mark != null) Destroy(mark);
        _tilledMarks.Remove(cell);
    }

    private void ApplyTintToMark(Vector3Int cell, Color tint)
    {
        if (!_tilledMarks.TryGetValue(cell, out GameObject mark)) return;
        if (mark == null) return;
        if (mark.TryGetComponent<SpriteRenderer>(out var sr))
            sr.color = tint;
    }
}
