// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenHappyState.cs
// Plays the heart animation when the player interacts
// with no tool equipped, then returns to Idle.
// ──────────────────────────────────────────────

public class ChickenHappyState : BaseAnimalState<ChickenNPC>
{
    public ChickenHappyState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Exit()
    {
    }
}
