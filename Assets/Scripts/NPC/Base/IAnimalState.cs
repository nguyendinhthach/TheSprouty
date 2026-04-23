// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/IAnimalState.cs
// Contract every animal behaviour state must fulfil.
// ──────────────────────────────────────────────

public interface IAnimalState
{
    /// <summary>Called once when the state machine enters this state.</summary>
    void Enter();

    /// <summary>Called every frame while this state is active.</summary>
    void Tick();

    /// <summary>Called once when the state machine leaves this state.</summary>
    void Exit();
}
