using UnityEngine;

public class SelectToolSlot : MonoBehaviour
{
    [SerializeField] private ToolSO tool;
    [SerializeField] private GameObject toolSlotVisual;
    [SerializeField] private GameObject toolSlotSelectedVisual;
    [SerializeField] private RadialToolWheelUI radialToolWheelUI;

    private void Start()
    {
        SetSelected(Player.Instance.EquippedToolSO == tool);
    }

    public void SelectTool()
    {
        Player.Instance.ChangeEquippedTool(tool);
        radialToolWheelUI.OnSlotSelected(this);
    }

    public void SetSelected(bool selected)
    {
        toolSlotVisual.SetActive(!selected);
        toolSlotSelectedVisual.SetActive(selected);
    }
}
