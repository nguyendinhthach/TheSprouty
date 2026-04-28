using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject itemSlotPrefab;
    [SerializeField] private Transform content;
    [SerializeField] private InventoryAnimator inventoryAnimator;

    [Header("Dependencies")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;

    private List<ItemSlotUI> _slotUIs = new List<ItemSlotUI>();
    private CanvasGroup _canvasGroup;
    private bool _isVisible = false;

    public static bool IsOpen { get; private set; }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        SetVisible(false);

        InventoryManager.Instance.OnInventoryChanged += InventoryManager_OnInventoryChanged;
        gameInput.OnToggleInventoryAction += GameInput_OnToggleInventoryAction;
        GenerateSlots();
        RefreshUI();
    }

    private void GameInput_OnToggleInventoryAction(object sender, EventArgs e)
    {
        if (RadialToolWheelUI.IsOpen) return;

        if (_isVisible)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void CloseInventory()
    {
        _isVisible = false;
        IsOpen = false;
        inventoryAnimator.Hide();
        playerIndicator.gameObject.SetActive(true);
    }   
    
    public void OpenInventory()
    {
        _isVisible = true;
        IsOpen = true;
        inventoryAnimator.Show();
        playerIndicator.gameObject.SetActive(false);
    }

    private void InventoryManager_OnInventoryChanged(object sender, EventArgs e)
    {
        RefreshUI();
    }

    private void OnDestroy()
    {
        InventoryManager.Instance.OnInventoryChanged -= InventoryManager_OnInventoryChanged;
        gameInput.OnToggleInventoryAction -= GameInput_OnToggleInventoryAction;
    }

    private void GenerateSlots()
    {
        for (int i = 0; i < InventoryManager.Instance.GetSlots().Length; i++)
        {
            ItemSlotUI slotUI = Instantiate(itemSlotPrefab, content)
                                .GetComponent<ItemSlotUI>();
            slotUI.Setup(i);
            _slotUIs.Add(slotUI);
        }
    }

    private void RefreshUI()
    {
        ItemSlot[] slots = InventoryManager.Instance.GetSlots();

        for (int i = 0; i < _slotUIs.Count; i++)
        {
            _slotUIs[i].Bind(slots[i]);
        }
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1 : 0;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }
}
