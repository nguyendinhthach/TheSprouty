using UnityEngine;

[CreateAssetMenu(fileName = "New Tool", menuName = "TheSprouty/Items/Tool")]
public class ToolSO : BaseItemSO
{
    [Header("Tool-Specific Data")]
    public ToolType toolType;
    [Tooltip("Half-size of the interact area in grid cells. 1 = 3x3, 2 = 5x5")]
    public int interactRange = 1;
    public int power = 10;
}
