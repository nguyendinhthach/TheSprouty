using System;
using UnityEngine;

public class RadialToolWheelUI : MonoBehaviour
{
    [SerializeField] private SelectToolSlot[] toolSlots;

    [SerializeField] private GameInput gameInput;

    [SerializeField] private PlayerIndicator playerIndicator;

    [SerializeField] private RadialWheelAnimator wheelAnimator;

    private void Start()
    {
        gameInput.OnToggleToolWheelAction += GameInput_OnToggleToolWheelAction;
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        gameInput.OnToggleToolWheelAction -= GameInput_OnToggleToolWheelAction;
    }

    private void GameInput_OnToggleToolWheelAction(object sender, System.EventArgs e)
    {
        if (gameObject.activeSelf)
        {
            wheelAnimator.Hide();
            playerIndicator.gameObject.SetActive(true);
        }
        else
        {
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
}
