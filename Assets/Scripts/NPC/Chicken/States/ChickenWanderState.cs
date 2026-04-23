// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenWanderState.cs
// Chicken walks to a random NavMesh point then returns to Idle.
// ──────────────────────────────────────────────

public class ChickenWanderState : BaseAnimalState<ChickenNPC>
{
    public ChickenWanderState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        Owner.Agent.speed = Owner.AnimalData.moveSpeed;
        Owner.ResumeAgent();

        if (Owner.TryGetRandomWanderPoint(out UnityEngine.Vector3 target))
            Owner.Agent.SetDestination(target);
        else
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }

    public override void Tick()
    {
        if (HasArrived())
            Owner.StateMachine.ChangeState(Owner.IdleState);
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
