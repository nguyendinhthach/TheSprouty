// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenWalkToNestState.cs
// Chicken walks toward the nest. When arrived,
// transitions to JumpIntoNest.
// ──────────────────────────────────────────────

public class ChickenWalkToNestState : BaseAnimalState<ChickenNPC>
{
    public ChickenWalkToNestState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        // If no nest assigned, bail out immediately
        if (Owner.NestZonePosition == (UnityEngine.Vector2)Owner.transform.position)
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Tick()
    {
        bool arrived = Owner.MoveToward(Owner.NestZonePosition);
        if (arrived)
            Owner.StateMachine.ChangeState(Owner.JumpIntoNestState);
    }
}
