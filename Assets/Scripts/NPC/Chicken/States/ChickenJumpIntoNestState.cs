// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenJumpIntoNestState.cs
// Plays the jump-into-nest animation once, then transitions to SleepInNest.
// ──────────────────────────────────────────────

public class ChickenJumpIntoNestState : BaseAnimalState<ChickenNPC>
{
    public ChickenJumpIntoNestState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.SleepInNestState);
    }
}
