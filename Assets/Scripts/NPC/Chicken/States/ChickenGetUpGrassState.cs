// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenGetUpGrassState.cs
// Plays the get-up animation once, then returns to Idle.
// ──────────────────────────────────────────────

public class ChickenGetUpGrassState : BaseAnimalState<ChickenNPC>
{
    public ChickenGetUpGrassState(ChickenNPC owner) : base(owner) { }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Exit()
    {
    }
}
