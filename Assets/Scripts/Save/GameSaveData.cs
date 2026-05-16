// ──────────────────────────────────────────────
// TheSprouty | Scripts/Save/GameSaveData.cs
// Root save object and all per-system save data classes.
// All classes are pure data — no MonoBehaviour, no Unity references.
// Serialized to / from JSON via JsonUtility.
// ──────────────────────────────────────────────
using System;
using System.Collections.Generic;

// ── Time ──────────────────────────────────────────────────────────────────────

[Serializable]
public class TimeSaveData
{
    /// <summary>Day number at time of save (starts at 1).</summary>
    public int currentDay;
}

// ── Player ────────────────────────────────────────────────────────────────────

[Serializable]
public class PlayerSaveData
{
    /// <summary>World position of the player when saved.</summary>
    public SerializableVector3 position;
}

// ── Inventory ─────────────────────────────────────────────────────────────────

[Serializable]
public class ItemSlotSaveData
{
    /// <summary>
    /// Asset name of the ItemSO (matches ScriptableObject.name).
    /// Empty string means the slot is empty.
    /// </summary>
    public string itemName;

    public int quantity;
}

[Serializable]
public class InventorySaveData
{
    /// <summary>
    /// One entry per inventory slot in order.
    /// Empty slots are stored with itemName = "" so slot count is preserved.
    /// </summary>
    public List<ItemSlotSaveData> slots = new List<ItemSlotSaveData>();
}

// ── Farm Tiles ────────────────────────────────────────────────────────────────

[Serializable]
public class FarmCellSaveData
{
    /// <summary>Cell coordinate on the Tilemap.</summary>
    public SerializableVector3Int cell;

    /// <summary>FarmTileState cast to int.</summary>
    public int state;

    /// <summary>Whether this cell was watered today.</summary>
    public bool isWatered;

    /// <summary>
    /// Total in-game hour when this Dirt cell was created.
    /// Only meaningful when state == Dirt (1); -1 otherwise.
    /// </summary>
    public int dirtCreatedAtHour;
}

// ── Crops ─────────────────────────────────────────────────────────────────────

[Serializable]
public class CropSaveData
{
    /// <summary>Cell this crop occupies.</summary>
    public SerializableVector3Int cell;

    /// <summary>Asset name of the CropDataSO.</summary>
    public string cropDataName;

    public int   currentStageIndex;
    public float hoursAccumulated;
    public bool  isMature;
}

// ── Farm (tiles + crops combined) ─────────────────────────────────────────────

[Serializable]
public class FarmSaveData
{
    /// <summary>Only cells that differ from the default GrassDirt state are stored.</summary>
    public List<FarmCellSaveData> cells = new List<FarmCellSaveData>();

    /// <summary>All live CropObjects at the time of save.</summary>
    public List<CropSaveData> crops = new List<CropSaveData>();
}

// ── Resource Nodes ────────────────────────────────────────────────────────────

[Serializable]
public class ResourceNodeSaveData
{
    /// <summary>Stable scene identifier set in the Inspector on each ResourceNode.</summary>
    public string nodeID;

    /// <summary>
    /// Health remaining. 0 means the node was destroyed
    /// and should not be present in the scene on load.
    /// </summary>
    public int currentHealth;

    /// <summary>
    /// FruitTree only: false when fruit has already dropped but tree is still alive.
    /// Ignored by all other node types.
    /// </summary>
    public bool hasFruit;
}

// ── NPCs ──────────────────────────────────────────────────────────────────────

[Serializable]
public class NPCSaveData
{
    /// <summary>Stable scene identifier set in the Inspector on each NPC.</summary>
    public string npcID;

    /// <summary>World position to restore on load.</summary>
    public SerializableVector3 position;

    /// <summary>
    /// In-game hours remaining until next item production (egg/milk).
    /// Persisted so the timer doesn't reset to a random value every session.
    /// </summary>
    public float productionHoursRemaining;

    /// <summary>
    /// True if the animal was fed this production cycle and the timer is actively counting.
    /// Persisted so the timer resumes correctly after load without requiring a re-feed.
    /// </summary>
    public bool canProduce;
}

// ── World Items (dropped, not yet collected) ──────────────────────────────────

[Serializable]
public class WorldItemSaveData
{
    /// <summary>Asset name of the ItemSO (matches ScriptableObject.name).</summary>
    public string itemName;

    /// <summary>World position where the item was lying on the ground.</summary>
    public SerializableVector3 position;

    /// <summary>
    /// In-game days remaining before this item despawns.
    /// Already accounts for the upcoming day advance (saved as DaysRemaining - 1).
    /// </summary>
    public int daysRemaining;
}

// ── Root save object ──────────────────────────────────────────────────────────

[Serializable]
public class GameSaveData
{
    /// <summary>Bumped whenever the save format changes. Used for migration.</summary>
    public int saveVersion = 1;

    public TimeSaveData               time          = new TimeSaveData();
    public PlayerSaveData             player        = new PlayerSaveData();
    public InventorySaveData          inventory     = new InventorySaveData();
    public FarmSaveData               farm          = new FarmSaveData();
    public List<ResourceNodeSaveData> resourceNodes = new List<ResourceNodeSaveData>();
    public List<NPCSaveData>          npcs          = new List<NPCSaveData>();
    public List<WorldItemSaveData>    worldItems    = new List<WorldItemSaveData>();
    public int                        gold          = 0;
}
