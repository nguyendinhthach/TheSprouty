// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowLayDownState.cs
// One-shot animation: cow sits down → transitions to SitIdle.
// ──────────────────────────────────────────────

public class CowLayDownState : BaseAnimalState<CowNPC>
{
    public CowLayDownState(CowNPC owner) : base(owner) { }

    public override void Enter()
    {
        Owner.StopAgent();
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.SitIdleState);
    }
}
