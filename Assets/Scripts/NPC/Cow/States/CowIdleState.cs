// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/States/CowIdleState.cs
// Cow stands still. Randomly plays BlinkIdle or TailWagIdle.
// Occasionally plays Sniff/Graze sub-actions.
// ──────────────────────────────────────────────
using UnityEngine;

public class CowIdleState : BaseAnimalState<CowNPC>
{
    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    /// <summary>True → BlinkIdle. False → TailWagIdle.</summary>
    public bool UseBlinkVariant { get; private set; }

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private float _idleTimer;
    private float _subActionTimer;

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------
    public CowIdleState(CowNPC owner) : base(owner) { }

    // ----------------------------------------------------------
    // IAnimalState
    // ----------------------------------------------------------
    public override void Enter()
    {
        UseBlinkVariant = Random.value > 0.5f;
        _idleTimer      = Random.Range(Owner.AnimalData.idleTimeMin, Owner.AnimalData.idleTimeMax);
        ResetSubActionTimer();
        Owner.StopAgent();
    }

    public override void Tick()
    {
        _idleTimer      -= Time.deltaTime;
        _subActionTimer -= Time.deltaTime;

        TryPlaySubAction();

        if (_idleTimer > 0f) return;
        if (Owner.IsPlayingSubAction) return; // chờ Sniff/Graze xong mới transition

        TransitionToNextState();
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void TryPlaySubAction()
    {
        if (_subActionTimer > 0f || Owner.IsPlayingSubAction) return;
        if (Owner.CowData == null) return;

        if (Random.value < Owner.CowData.subActionChance)
            Owner.PlaySubAction(Random.value > 0.5f); // true = Sniff, false = Graze

        ResetSubActionTimer();
    }

    private void ResetSubActionTimer()
    {
        if (Owner.CowData == null) return;
        _subActionTimer = Random.Range(
            Owner.CowData.subActionIntervalMin,
            Owner.CowData.subActionIntervalMax);
    }

    private void TransitionToNextState()
    {
        float rand = Random.value;
        if (rand < Owner.CowData.sleepChance)
            Owner.StateMachine.ChangeState(Owner.LayDownState);
        else
            Owner.StateMachine.ChangeState(Owner.WanderState);
    }
}
