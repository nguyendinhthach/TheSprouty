// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowGetUpState.cs
// One-shot animation: cow stands up → Idle.
// ──────────────────────────────────────────────

public class CowGetUpState : BaseAnimalState<CowNPC>
{
    public CowGetUpState(CowNPC owner) : base(owner) { }

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
