using System;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public event EventHandler OnInventoryChanged;

    public static InventoryManager Instance { get; private set; }

    [SerializeField] private int slotCount;
    private ItemSlot[] itemSlots;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Multiple InventoryManager instances detected. Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
        Instance = this;

        itemSlots = new ItemSlot[slotCount];
        for (int i = 0; i < slotCount; i++)
            itemSlots[i] = new ItemSlot();
    }

    public ItemSlot[] GetSlots() => itemSlots;

    public bool AddItem(ItemSO itemSO, int quantity)
    {
        // Pass 1: stack vào slot đã có cùng item
        foreach (var slot in itemSlots)
        {
            if (!slot.IsEmpty && slot.GetItemSO() == itemSO)
            {
                quantity = slot.AddQuantity(quantity);
                if (quantity <= 0)
                {
                    OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }
        }

        // Pass 2: tìm slot trống
        foreach (var slot in itemSlots)
        {
            if (slot.IsEmpty)
            {
                slot.SetItem(itemSO);
                quantity = slot.AddQuantity(quantity);
                if (quantity <= 0)
                {
                    OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                    return true;
                }
            }
        }

        return false;
    }

    public bool RemoveItem(ItemSO itemSO, int quantity)
    {
        foreach (var slot in itemSlots)
        {
            if (!slot.IsEmpty && slot.GetItemSO() == itemSO)
            {
                int remainingQuantity = slot.AddQuantity(-quantity);
                if (remainingQuantity <= 0)
                {
                    slot.ClearSlot();
                }
                OnInventoryChanged?.Invoke(this, EventArgs.Empty);
                return true;
            }
        }
        Debug.LogWarning("Item not found in inventory.");
        return false;
    }

    public bool HasItem(ItemSO itemSO)
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.GetItemSO() == itemSO)
            {
                return true;
            }
        }
        return false;
    }

    public int GetItemQuantity(ItemSO itemSO)
    {
        foreach (ItemSlot itemSlot in itemSlots)
        {
            if (itemSlot.GetItemSO() == itemSO)
            {
                return itemSlot.GetQuantity();
            }
        }
        return 0;
    }

    public bool HasSpace(ItemSO itemSO)
    {
        foreach (var slot in itemSlots)
            if (!slot.IsEmpty && slot.GetItemSO() == itemSO
                && slot.GetQuantity() < itemSO.maxStackSize)
                return true;

        foreach (var slot in itemSlots)
            if (slot.IsEmpty) return true;

        return false;
    }

    public void SwapSlots(int indexA, int indexB)
    {
        (itemSlots[indexA], itemSlots[indexB]) = (itemSlots[indexB], itemSlots[indexA]);
        OnInventoryChanged?.Invoke(this, EventArgs.Empty);
    }
}
