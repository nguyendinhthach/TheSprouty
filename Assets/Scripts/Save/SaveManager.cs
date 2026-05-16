// ──────────────────────────────────────────────
// TheSprouty | Scripts/Save/SaveManager.cs
// Central save/load coordinator.
// Hooks into DayCycleManager sleep events.
//
// STEP 1: hooks wired, Debug.Log only.
// STEP 2: ItemRegistrySO wired in.
// STEP 3: Time, Player, Inventory capture/restore.
// STEP 4: Farm + Crop capture/restore.
// STEP 5: ResourceNode capture/restore.
// STEP 6+: NPCs.
// ──────────────────────────────────────────────
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Runs after all default-order scripts (ResourceNode, FarmTileManager, etc.)
// so their Start() methods initialize before we call RestoreAll().
[DefaultExecutionOrder(100)]
public class SaveManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------

    /// <summary>Fired just before writing to disk. Use to show saving indicator.</summary>
    public static event Action OnSaveStarted;

    /// <summary>Fired after file has been written. Use to hide saving indicator.</summary>
    public static event Action OnSaveCompleted;

    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static SaveManager Instance { get; private set; }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private ItemRegistrySO  itemRegistry;
    [SerializeField] private FarmTileManager farmTileManager;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private const int    SAVE_VERSION   = 1;
    private const string SAVE_FILE_NAME = "sprouty_save.json";

    /// <summary>IDs of nodes destroyed during this play session.</summary>
    private readonly HashSet<string> _destroyedNodeIDs = new HashSet<string>();

    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        if (itemRegistry == null)
            Debug.LogError("[SaveManager] ItemRegistry is not assigned!");
        if (farmTileManager == null)
            Debug.LogError("[SaveManager] FarmTileManager is not assigned!");
    }

    private void Start()
    {
        DayCycleManager.Instance.OnSleepStarted += HandleSleepStarted;
        DayCycleManager.Instance.OnSleepEnded   += HandleSleepEnded;
        ResourceNode.OnNodeDestroyed            += HandleNodeDestroyed;

        // ── Load on game start ──
        if (File.Exists(SaveFilePath))
        {
            Debug.Log("[SaveManager] Save file found → loading...");
            LoadGame();
        }
        else
        {
            Debug.Log("[SaveManager] No save file — starting fresh.");
        }
    }

    private void OnDestroy()
    {
        if (DayCycleManager.Instance != null)
        {
            DayCycleManager.Instance.OnSleepStarted -= HandleSleepStarted;
            DayCycleManager.Instance.OnSleepEnded   -= HandleSleepEnded;
        }
        ResourceNode.OnNodeDestroyed -= HandleNodeDestroyed;
    }

    // ----------------------------------------------------------
    // Event handlers
    // ----------------------------------------------------------
    private void HandleSleepStarted(object sender, EventArgs e)
    {
        // Save happens at OnSleepEnded (not here) so crops, watered state,
        // and worldItem daysRemaining have all been updated by OnDayPassed first.
    }

    private void HandleSleepEnded(object sender, EventArgs e)
    {
        // Day has advanced, OnDayPassed has fired, crops have grown through the night.
        // This is the correct moment to capture the true post-sleep state.
        Debug.Log("[SaveManager] Saving...");
        SaveGame();
    }

    private void HandleNodeDestroyed(string id)
    {
        _destroyedNodeIDs.Add(id);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Captures all system states and writes to disk.</summary>
    public void SaveGame()
    {
        OnSaveStarted?.Invoke();
        GameSaveData data = CaptureAll();
        WriteToFile(data);
        OnSaveCompleted?.Invoke();
        Debug.Log($"[SaveManager] Saved day {data.time.currentDay} → {SaveFilePath}");
    }

    /// <summary>Reads save file from disk and restores all system states.</summary>
    public void LoadGame()
    {
        GameSaveData data = ReadFromFile();
        if (data == null) return;

        RestoreAll(data);
        Debug.Log($"[SaveManager] Loaded day {data.time.currentDay}");
    }

    /// <summary>Deletes the save file. Useful for debug / new-game flow.</summary>
    public void DeleteSave()
    {
        if (!File.Exists(SaveFilePath)) return;
        File.Delete(SaveFilePath);
        Debug.Log("[SaveManager] Save file deleted.");
    }

    // ----------------------------------------------------------
    // Private — capture
    // ----------------------------------------------------------
    private GameSaveData CaptureAll()
    {
        GameSaveData data = new GameSaveData();

        data.time          = CaptureTime();
        data.player        = CapturePlayer();
        data.inventory     = CaptureInventory();
        data.farm          = CaptureFarm();
        data.resourceNodes = CaptureResourceNodes();
        data.npcs          = CaptureNPCs();
        data.worldItems    = CaptureWorldItems();
        data.gold          = EconomyManager.Instance.Gold;

        return data;
    }

    private TimeSaveData CaptureTime()
    {
        // Save is called at OnSleepEnded — day has already advanced.
        // No +1 needed.
        return new TimeSaveData
        {
            currentDay = DayCycleManager.Instance.CurrentDay
        };
    }

    private PlayerSaveData CapturePlayer()
    {
        return new PlayerSaveData
        {
            position = Player.Instance.transform.position
        };
    }

    private InventorySaveData CaptureInventory()
    {
        InventorySaveData data  = new InventorySaveData();
        ItemSlot[]        slots = InventoryManager.Instance.GetSlots();

        foreach (ItemSlot slot in slots)
        {
            data.slots.Add(new ItemSlotSaveData
            {
                itemName = slot.IsEmpty ? "" : slot.GetItemSO().name,
                quantity = slot.IsEmpty ? 0  : slot.GetQuantity()
            });
        }

        return data;
    }

    private FarmSaveData CaptureFarm()
    {
        return farmTileManager.CaptureFarmData();
    }

    private List<ResourceNodeSaveData> CaptureResourceNodes()
    {
        List<ResourceNodeSaveData> list = new List<ResourceNodeSaveData>();

        // ── Living nodes with non-default state ──
        ResourceNode[] allNodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
        foreach (ResourceNode node in allNodes)
        {
            if (string.IsNullOrEmpty(node.NodeID))
            {
                Debug.LogWarning($"[SaveManager] ResourceNode '{node.name}' has no NodeID — skipped.");
                continue;
            }

            bool isDamaged  = node.CurrentHealth < node.MaxHealth;
            bool lostFruit  = !node.HasFruit;

            if (!isDamaged && !lostFruit) continue;  // still default — skip

            list.Add(new ResourceNodeSaveData
            {
                nodeID        = node.NodeID,
                currentHealth = node.CurrentHealth,
                hasFruit      = node.HasFruit
            });
        }

        // ── Destroyed nodes (no longer in scene) ──
        foreach (string id in _destroyedNodeIDs)
        {
            list.Add(new ResourceNodeSaveData
            {
                nodeID        = id,
                currentHealth = 0,
                hasFruit      = false
            });
        }

        return list;
    }

    // ----------------------------------------------------------
    // Private — restore
    // ----------------------------------------------------------
    private void RestoreAll(GameSaveData data)
    {
        RestoreTime(data.time);
        // Player position: BedInteractable.Start() handles wakeUpPoint placement.
        RestoreInventory(data.inventory);
        RestoreFarm(data.farm);
        RestoreResourceNodes(data.resourceNodes);
        RestoreNPCs(data.npcs);
        RestoreWorldItems(data.worldItems);
        EconomyManager.Instance.LoadGold(data.gold);
    }

    private void RestoreTime(TimeSaveData data)
    {
        DayCycleManager.Instance.LoadDay(data.currentDay);
    }

    private void RestoreInventory(InventorySaveData data)
    {
        InventoryManager.Instance.LoadFromSave(data.slots, itemRegistry);
    }

    private void RestoreFarm(FarmSaveData data)
    {
        farmTileManager.LoadFromSave(data, itemRegistry);
    }

    private List<WorldItemSaveData> CaptureWorldItems()
    {
        List<WorldItemSaveData> list       = new List<WorldItemSaveData>();
        ItemPickup[]            allPickups = FindObjectsByType<ItemPickup>(FindObjectsSortMode.None);

        foreach (ItemPickup pickup in allPickups)
        {
            // Skip items still mid-bounce — they haven't landed yet
            if (!pickup.CanBePickedUp) continue;

            WorldItem worldItem = pickup.GetComponent<WorldItem>();
            if (worldItem == null) continue;

            ItemSO item = worldItem.GetItemSO();
            if (item == null) continue;

            // Save is called at OnSleepEnded — OnDayPassed has already
            // decremented daysRemaining. Save the current value directly.
            int savedDays = worldItem.DaysRemaining;
            if (savedDays <= 0) continue;

            list.Add(new WorldItemSaveData
            {
                itemName      = item.name,
                position      = pickup.transform.position,
                daysRemaining = savedDays
            });
        }

        return list;
    }

    private void RestoreWorldItems(List<WorldItemSaveData> savedItems)
    {
        if (savedItems == null || savedItems.Count == 0) return;

        foreach (WorldItemSaveData saved in savedItems)
        {
            ItemSO item = itemRegistry.GetItem(saved.itemName);
            if (item == null || item.prefab == null)
            {
                Debug.LogWarning($"[SaveManager] WorldItem '{saved.itemName}': prefab not found.");
                continue;
            }

            Transform spawned = Instantiate(item.prefab, saved.position, Quaternion.identity);

            // Item is already on the ground — skip bounce, enable pickup immediately
            if (spawned.TryGetComponent<ItemBounceObject>(out var bounce))
                bounce.enabled = false;

            if (spawned.TryGetComponent<ItemPickup>(out var pickup))
                pickup.CanBePickedUp = true;

            // Restore despawn counter
            if (spawned.TryGetComponent<WorldItem>(out var worldItem))
                worldItem.SetDaysRemaining(saved.daysRemaining);
        }
    }

    private List<NPCSaveData> CaptureNPCs()
    {
        List<NPCSaveData>  list    = new List<NPCSaveData>();
        BaseAnimalNPC[]    allNPCs = FindObjectsByType<BaseAnimalNPC>(FindObjectsSortMode.None);

        foreach (BaseAnimalNPC npc in allNPCs)
        {
            if (string.IsNullOrEmpty(npc.NPCID))
            {
                Debug.LogWarning($"[SaveManager] NPC '{npc.name}' has no NPCID — skipped.");
                continue;
            }

            list.Add(new NPCSaveData
            {
                npcID                    = npc.NPCID,
                position                 = npc.transform.position,
                productionHoursRemaining = npc.ProductionHoursRemaining,
                canProduce               = npc.CanProduce
            });
        }

        return list;
    }

    private void RestoreNPCs(List<NPCSaveData> savedNPCs)
    {
        if (savedNPCs == null || savedNPCs.Count == 0) return;

        BaseAnimalNPC[] allNPCs = FindObjectsByType<BaseAnimalNPC>(FindObjectsSortMode.None);
        Dictionary<string, BaseAnimalNPC> npcMap = new Dictionary<string, BaseAnimalNPC>();
        foreach (BaseAnimalNPC npc in allNPCs)
            if (!string.IsNullOrEmpty(npc.NPCID))
                npcMap[npc.NPCID] = npc;

        foreach (NPCSaveData saved in savedNPCs)
        {
            if (!npcMap.TryGetValue(saved.npcID, out BaseAnimalNPC npc))
            {
                Debug.LogWarning($"[SaveManager] NPC '{saved.npcID}' not found in scene.");
                continue;
            }

            npc.LoadNPCState(saved.position, saved.productionHoursRemaining, saved.canProduce);
        }
    }

    private void RestoreResourceNodes(List<ResourceNodeSaveData> savedNodes)
    {
        if (savedNodes == null || savedNodes.Count == 0) return;

        // Build lookup map: nodeID → ResourceNode (scene objects)
        ResourceNode[] allNodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
        Dictionary<string, ResourceNode> nodeMap = new Dictionary<string, ResourceNode>();
        foreach (ResourceNode node in allNodes)
            if (!string.IsNullOrEmpty(node.NodeID))
                nodeMap[node.NodeID] = node;

        foreach (ResourceNodeSaveData saved in savedNodes)
        {
            if (!nodeMap.TryGetValue(saved.nodeID, out ResourceNode node))
            {
                Debug.LogWarning($"[SaveManager] Node '{saved.nodeID}' not found in scene.");
                continue;
            }

            if (saved.currentHealth <= 0)
            {
                // Was destroyed — remove silently (no drops, no effects).
                // Also re-register in _destroyedNodeIDs so subsequent saves still track it.
                Destroy(node.gameObject);
                _destroyedNodeIDs.Add(saved.nodeID);
            }
            else
            {
                node.LoadNodeState(saved.currentHealth);

                if (!saved.hasFruit)
                    node.LoadFruitlessState();
            }
        }
    }

    // ----------------------------------------------------------
    // Private — file I/O
    // ----------------------------------------------------------
    private void WriteToFile(GameSaveData data)
    {
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(SaveFilePath, json);
    }

    private GameSaveData ReadFromFile()
    {
        if (!File.Exists(SaveFilePath))
        {
            Debug.LogWarning("[SaveManager] ReadFromFile: file not found.");
            return null;
        }

        string json = File.ReadAllText(SaveFilePath);
        GameSaveData data = JsonUtility.FromJson<GameSaveData>(json);

        if (data.saveVersion != SAVE_VERSION)
            Debug.LogWarning($"[SaveManager] Version mismatch: file={data.saveVersion}, expected={SAVE_VERSION}.");

        return data;
    }
}
