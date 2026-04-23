// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/Nest.cs
// Quản lý trạng thái của tổ gà.
// Chỉ 1 gà được chiếm tổ tại 1 thời điểm.
// ──────────────────────────────────────────────
using UnityEngine;

public class Nest : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private Transform nestPoint;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public Transform NestPoint  => nestPoint;
    public bool      IsOccupied { get; private set; }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>
    /// Thử chiếm tổ. Trả về true nếu thành công (tổ trống),
    /// false nếu tổ đã có gà.
    /// </summary>
    public bool TryOccupy()
    {
        if (IsOccupied) return false;
        IsOccupied = true;
        return true;
    }

    /// <summary>Giải phóng tổ khi gà rời đi.</summary>
    public void Vacate()
    {
        IsOccupied = false;
    }
}
