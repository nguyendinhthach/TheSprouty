// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/BaseAnimalState.cs
// Generic base state — Owner is typed to the concrete NPC
// so each subclass gets full type-safe access without casts.
// ──────────────────────────────────────────────

public abstract class BaseAnimalState<T> : IAnimalState where T : BaseAnimalNPC
{
    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------

    /// <summary>The owning NPC. Typed to T for direct field/method access.</summary>
    protected T Owner { get; private set; }

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------

    protected BaseAnimalState(T owner)
    {
        Owner = owner;
    }

    // ----------------------------------------------------------
    // IAnimalState  — virtual so subclasses override only what they need
    // ----------------------------------------------------------
    public virtual void Enter() { }
    public abstract void Tick();
    public virtual void Exit() { }
}
