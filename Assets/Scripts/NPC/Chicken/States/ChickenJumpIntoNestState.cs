// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenJumpIntoNestState.cs
// Plays the jump-into-nest animation once, then transitions to SleepInNest.
// ──────────────────────────────────────────────

public class ChickenJumpIntoNestState : BaseAnimalState<ChickenNPC>
{
    public ChickenJumpIntoNestState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        // Chiếm tổ — nếu bị chiếm mất lúc đang đi tới thì quay về Idle
        if (!Owner.TryOccupyNest())
        {
            Owner.StateMachine.ChangeState(Owner.IdleState);
            return;
        }
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.SleepInNestState);
    }
}
