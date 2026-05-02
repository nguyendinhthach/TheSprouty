using System;
using UnityEngine;

public class RadialToolWheelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SelectToolSlot[] toolSlots;
    [SerializeField] private RadialWheelAnimator wheelAnimator;
    [SerializeField] private RadialSeedWheelUI radialSeedWheelUI;

    [Header("Dependencies")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;

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
        gameInput.OnToggleToolWheelAction += GameInput_OnToggleToolWheelAction;
    }

    private void OnDestroy()
    {
        gameInput.OnToggleToolWheelAction -= GameInput_OnToggleToolWheelAction;
    }

    private void GameInput_OnToggleToolWheelAction(object sender, System.EventArgs e)
    {
        if (InventoryUI.IsOpen) return;

        // Nếu tầng 2 đang mở → đóng tầng 2, không toggle tầng 1
        if (RadialSeedWheelUI.IsOpen)
        {
            radialSeedWheelUI.Close();
            return;
        }

        if (_isVisible)
        {
            _isVisible = false;
            IsOpen = false;
            wheelAnimator.Hide();
            playerIndicator.gameObject.SetActive(true);
        }
        else
        {
            _isVisible = true;
            IsOpen = true;
            wheelAnimator.Show();
            playerIndicator.gameObject.SetActive(false);
        }
    }

    public void OnSlotSelected(SelectToolSlot selectedSlot)
    {
        foreach (SelectToolSlot slot in toolSlots)
        {
            slot.SetSelected(slot == selectedSlot);
        }
    }

    public void Close()
    {
        _isVisible = false;
        IsOpen = false;
        wheelAnimator.Hide();
        playerIndicator.gameObject.SetActive(true);
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1 : 0;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }
}
