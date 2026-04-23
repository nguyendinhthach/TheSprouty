// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenSleepGrassState.cs
// Chicken sleeps on the grass for a configurable duration,
// then transitions to GetUpGrass.
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenSleepGrassState : BaseAnimalState<ChickenNPC>
{
    private float _timer;

    public ChickenSleepGrassState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        _timer = Owner.ChickenData.sleepDuration;
    }

    public override void Tick()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            Owner.StateMachine.ChangeState(Owner.GetUpGrassState);
    }
}
