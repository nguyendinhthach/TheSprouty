// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenSleepInNestState.cs
// Chicken rests inside the nest for a configurable duration.
// Transitions to JumpOutNest when done.
// (Future: if egg is present → Incubating instead.)
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenSleepInNestState : BaseAnimalState<ChickenNPC>
{
    private float _timer;

    public ChickenSleepInNestState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        _timer = Owner.ChickenData.nestSleepDuration;
    }

    public override void Tick()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            // TODO: when egg system is ready, check for egg here
            // and transition to IncubatingState instead.
            Owner.StateMachine.ChangeState(Owner.JumpOutNestState);
        }
    }
}
