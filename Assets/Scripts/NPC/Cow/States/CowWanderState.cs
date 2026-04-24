// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowWanderState.cs
// Cow walks to a random NavMesh point then returns to Idle.
// ──────────────────────────────────────────────
using UnityEngine.AI;

public class CowWanderState : BaseAnimalState<CowNPC>
{
    public CowWanderState(CowNPC owner) : base(owner) { }

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
        if (Owner.Agent.pathPending) return false;
        // PathInvalid = path bị chặn → give up về Idle
        if (Owner.Agent.pathStatus == NavMeshPathStatus.PathInvalid) return true;
        // hasPath=false + remaining=0: agent đã xóa path sau khi arrive
        return Owner.Agent.remainingDistance <= Owner.Agent.stoppingDistance + 0.05f;
    }
}
