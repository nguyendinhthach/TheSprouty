// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenLayDownGrassState.cs
// Plays the lay-down animation once, then transitions to SleepGrass.
// ──────────────────────────────────────────────

public class ChickenLayDownGrassState : BaseAnimalState<ChickenNPC>
{
    public ChickenLayDownGrassState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
    }

    public override void Tick()
    {
        if (Owner.IsCurrentAnimationDone())
            Owner.StateMachine.ChangeState(Owner.SleepGrassState);
    }
}
