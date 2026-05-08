// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/AnimalNPCSO.cs
// Shared tunable data for all animal NPCs.
// Subclass this for animal-specific settings.
// ──────────────────────────────────────────────
using UnityEngine;

[CreateAssetMenu(fileName = "AnimalNPCData", menuName = "TheSprouty/NPC/Animal NPC Data")]
public class AnimalNPCSO : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float wanderRadiusMin = 1f;
    public float wanderRadiusMax = 4f;

    [Header("Idle")]
    public float idleTimeMin = 2f;
    public float idleTimeMax = 5f;

    [Header("Feeding")]
    [Tooltip("Item player cần có trong inventory để feed animal này (ví dụ: Wheat). Để trống = không cần feed.")]
    public ItemSO feedItem;
    [Tooltip("Số lượng feedItem tiêu thụ mỗi lần feed.")]
    public int    feedAmount = 1;

    [Header("Item Production")]
    [Tooltip("Item sẽ được spawn ra theo thời gian. Để trống = không sản xuất.")]
    public ItemSO productItem;
    [Tooltip("Số lượng item mỗi lần spawn.")]
    public int    productAmount         = 1;
    [Tooltip("Số giờ in-game tối thiểu giữa mỗi lần sản xuất.")]
    public float  productionHoursMin    = 8f;
    [Tooltip("Số giờ in-game tối đa giữa mỗi lần sản xuất.")]
    public float  productionHoursMax    = 12f;
}
