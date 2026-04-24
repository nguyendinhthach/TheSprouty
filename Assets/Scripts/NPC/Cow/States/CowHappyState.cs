// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowHappyState.cs
// Plays Happy animation when player interacts (no tool).
// ──────────────────────────────────────────────

public class CowHappyState : BaseAnimalState<CowNPC>
{
    public CowHappyState(CowNPC owner) : base(owner) { }

    public override void Enter()
    {
        Owner.StopAgent();
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Exit()
    {
        Owner.ResumeAgent();
    }
}
