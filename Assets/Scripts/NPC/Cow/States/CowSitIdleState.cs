// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowSitIdleState.cs
// Cow sits awake (SitBlinkIdle or SitTailWagIdle).
// After a random timer, decides to Sleep or GetUp.
// ──────────────────────────────────────────────
using UnityEngine;

public class CowSitIdleState : BaseAnimalState<CowNPC>
{
    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    /// <summary>True → SitBlinkIdle. False → SitTailWagIdle.</summary>
    public bool UseBlinkVariant { get; private set; }

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private float _timer;

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------
    public CowSitIdleState(CowNPC owner) : base(owner) { }

    // ----------------------------------------------------------
    // IAnimalState
    // ----------------------------------------------------------
    public override void Enter()
    {
        UseBlinkVariant = Random.value > 0.5f;
        _timer = Random.Range(Owner.CowData.sitIdleDurationMin, Owner.CowData.sitIdleDurationMax);
    }

    public override void Tick()
    {
        _timer -= UnityEngine.Time.deltaTime;
        if (_timer > 0f) return;

        // 60% chance to fall asleep, 40% chance to get up
        if (Random.value < 0.6f)
            Owner.StateMachine.ChangeState(Owner.SleepState);
        else
            Owner.StateMachine.ChangeState(Owner.GetUpState);
    }
}
