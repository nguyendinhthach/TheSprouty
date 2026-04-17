using UnityEngine;

public interface IDamageable
{
    void TakeDamage(ToolSO playerTool);
    bool IsAlive { get; }
}
