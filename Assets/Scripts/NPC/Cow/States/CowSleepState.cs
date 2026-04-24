// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowSleepState.cs
// Cow sleeps for a configurable duration → GetUp.
// ──────────────────────────────────────────────
using UnityEngine;

public class CowSleepState : BaseAnimalState<CowNPC>
{
    private float _timer;

    public CowSleepState(CowNPC owner) : base(owner) { }

    public override void Enter()
    {
        _timer = Owner.CowData.sleepDuration;
    }

    public override void Tick()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            Owner.StateMachine.ChangeState(Owner.GetUpState);
    }
}
