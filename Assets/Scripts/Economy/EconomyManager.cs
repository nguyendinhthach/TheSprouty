// ──────────────────────────────────────────────
// TheSprouty | Economy/EconomyManager.cs
// Manages player gold. Handles earn/spend transactions.
// ──────────────────────────────────────────────
using System;
using UnityEngine;

public class EconomyManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------
    public event EventHandler OnGoldChanged;

    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static EconomyManager Instance { get; private set; }

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private int _gold;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public int Gold => _gold;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------
    /// <summary>Adds gold to player wallet and fires OnGoldChanged.</summary>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;
        _gold += amount;
        OnGoldChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Restores gold from save data. Called by SaveManager on load.</summary>
    public void LoadGold(int savedGold)
    {
        _gold = savedGold;
        OnGoldChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Sells an item: adds gold and logs the transaction.
    /// Returns total gold earned.
    /// </summary>
    public int SellItem(ItemSO itemSO, int quantity)
    {
        int earned = itemSO.sellValue * quantity;
        AddGold(earned);
        Debug.Log($"[EconomyManager] Đã bán {itemSO.itemName} - số lượng {quantity} - thu được {earned} gold | Tổng: {_gold}g");
        return earned;
    }
}
