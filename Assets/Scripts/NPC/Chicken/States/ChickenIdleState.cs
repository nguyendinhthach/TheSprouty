// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenIdleState.cs
// Chicken stands still. Randomly picks BlinkIdle or
// LookAroundIdle, occasionally plays Eat sub-action,
// then transitions to Wander, SleepGrass, or Nest.
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenIdleState : BaseAnimalState<ChickenNPC>
{
    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------

    /// <summary>True → play BlinkIdle. False → play LookAroundIdle.</summary>
    public bool UseBlinkVariant { get; private set; }

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private float _idleTimer;
    private float _eatSubActionTimer;

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------
    public ChickenIdleState(ChickenNPC owner) : base(owner) { }

    // ----------------------------------------------------------
    // IAnimalState
    // ----------------------------------------------------------
    public override void Enter()
    {

        UseBlinkVariant  = Random.value > 0.5f;
        _idleTimer       = Random.Range(Owner.AnimalData.idleTimeMin, Owner.AnimalData.idleTimeMax);
        ResetEatTimer();
    }

    public override void Tick()
    {
        _idleTimer       -= Time.deltaTime;
        _eatSubActionTimer -= Time.deltaTime;

        TryPlayEatSubAction();

        if (_idleTimer > 0f) return;

        TransitionToNextState();
    }

    public override void Exit()
    {
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void TryPlayEatSubAction()
    {
        if (_eatSubActionTimer > 0f) return;
        if (Owner.ChickenData == null) return;
        if (!Owner.IsPlayingEatSubAction &&
            Random.value < Owner.ChickenData.eatSubActionChance)
        {
            Owner.PlayEatSubAction();
        }
        ResetEatTimer();
    }

    private void ResetEatTimer()
    {
        _eatSubActionTimer = Random.Range(
            Owner.ChickenData.eatSubActionIntervalMin,
            Owner.ChickenData.eatSubActionIntervalMax);
    }

    private void TransitionToNextState()
    {
        float rand = Random.value;

        // nestGoChance: gà quyết định đi đến nest (không cần phải đang ở gần)
        if (rand < Owner.ChickenData.nestGoChance && Owner.HasAnyNestAvailable)
        {
            Owner.StateMachine.ChangeState(Owner.WalkToNestState);
        }
        else if (rand < Owner.ChickenData.nestGoChance + Owner.ChickenData.sleepChance)
        {
            Owner.StateMachine.ChangeState(Owner.LayDownGrassState);
        }
        else
        {
            Owner.StateMachine.ChangeState(Owner.WanderState);
        }
    }
}
