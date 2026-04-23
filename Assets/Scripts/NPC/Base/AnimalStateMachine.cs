// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/AnimalStateMachine.cs
// Generic FSM shared by all animal NPCs.
// ──────────────────────────────────────────────

public class AnimalStateMachine
{
    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public IAnimalState CurrentState { get; private set; }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Set the starting state and call its Enter().</summary>
    public void Initialize(IAnimalState startState)
    {
        CurrentState = startState;
        CurrentState.Enter();
    }

    /// <summary>Exit current state, swap, then Enter the new one.</summary>
    public void ChangeState(IAnimalState newState)
    {
        CurrentState?.Exit();
        CurrentState = newState;
        CurrentState.Enter();
    }

    /// <summary>Forward the frame tick to the active state.</summary>
    public void Tick()
    {
        CurrentState?.Tick();
    }
}
