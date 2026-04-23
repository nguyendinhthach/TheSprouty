// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenJumpOutNestState.cs
// Plays the jump-out-of-nest animation once, then returns to Idle.
// ──────────────────────────────────────────────

public class ChickenJumpOutNestState : BaseAnimalState<ChickenNPC>
{
    public ChickenJumpOutNestState(ChickenNPC owner) : base(owner) { }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Exit()
    {
        Owner.VacateNest();
    }
}
