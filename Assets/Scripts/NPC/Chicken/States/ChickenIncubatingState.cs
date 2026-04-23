// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenIncubatingState.cs
// Chicken sits in the nest incubating an egg.
// Egg + chick logic is deferred — animation placeholder only.
// ──────────────────────────────────────────────

public class ChickenIncubatingState : BaseAnimalState<ChickenNPC>
{
    public ChickenIncubatingState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
    }

    public override void Tick()
    {
        // TODO: Implement egg incubation logic when the egg system is ready.
        // On completion → Owner.StateMachine.ChangeState(Owner.JumpOutNestState);
    }
}
