// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenWanderState.cs
// Chicken walks to a random point inside the wander bounds.
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenWanderState : BaseAnimalState<ChickenNPC>
{
    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Vector2 _target;

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------
    public ChickenWanderState(ChickenNPC owner) : base(owner) { }

    // ----------------------------------------------------------
    // IAnimalState
    // ----------------------------------------------------------
    public override void Enter()
    {
        _target = Owner.GetRandomWanderPoint();
    }

    public override void Tick()
    {
        bool arrived = Owner.MoveToward(_target);
        if (arrived)
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }
}
