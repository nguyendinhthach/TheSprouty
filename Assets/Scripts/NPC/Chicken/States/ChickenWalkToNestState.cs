// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenWalkToNestState.cs
// Chicken picks an available nest and walks to it.
// Aborts if nest becomes occupied while walking.
// ──────────────────────────────────────────────

public class ChickenWalkToNestState : BaseAnimalState<ChickenNPC>
{
    public ChickenWalkToNestState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        // Chọn tổ trống — nếu không có thì quay về Idle
        if (!Owner.SelectAvailableNest())
        {
            Owner.StateMachine.ChangeState(Owner.IdleState);
            return;
        }

        Owner.Agent.speed = Owner.AnimalData.moveSpeed;
        Owner.ResumeAgent();
        Owner.Agent.SetDestination(Owner.NestZonePosition);
    }

    public override void Tick()
    {
        // Tổ bị chiếm trong lúc đi → thử chọn tổ khác hoặc quay về Idle
        if (!Owner.IsTargetNestAvailable)
        {
            if (Owner.SelectAvailableNest())
                Owner.Agent.SetDestination(Owner.NestZonePosition);
            else
                Owner.StateMachine.ChangeState(Owner.IdleState);
            return;
        }

        if (HasArrived())
            Owner.StateMachine.ChangeState(Owner.JumpIntoNestState);
    }

    public override void Exit()
    {
        Owner.StopAgent();
    }

    private bool HasArrived()
    {
        return Owner.Agent.hasPath
            && !Owner.Agent.pathPending
            && Owner.Agent.remainingDistance <= Owner.Agent.stoppingDistance;
    }
}
