using UnityEngine;

[System.Serializable]
public class ItemSlot
{
    private ItemSO _itemSO;
    private int _quantity;

    public bool IsEmpty => _itemSO == null || _quantity <= 0;

    public void SetItem(ItemSO newItem)
    {
        _itemSO = newItem;
        _quantity = 0;
    }

    public int AddQuantity(int amount)
    {
        if (amount < 0)
        {
            int canRemove = Mathf.Min(-amount, _quantity);
            _quantity -= canRemove;
            return amount + canRemove; // phần còn thiếu nếu không đủ
        }
        int space = _itemSO.maxStackSize - _quantity;
        int canAdd = Mathf.Min(amount, space);
        _quantity += canAdd;
        return amount - canAdd;
    }


    public void ClearSlot()
    {
        _itemSO = null;
        _quantity = 0;
    }

    public ItemSO GetItemSO() => _itemSO;

    public int GetQuantity() => _quantity;
}
