// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenFleeState.cs
// Chicken flees away from the player by lerping to a
// calculated flee position over the clip's duration.
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenFleeState : BaseAnimalState<ChickenNPC>
{
    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private float   _elapsed;
    private Vector2 _startPos;
    private Vector2 _targetPos;

    // ----------------------------------------------------------
    // Constructor
    // ----------------------------------------------------------
    public ChickenFleeState(ChickenNPC owner) : base(owner) { }

    // ----------------------------------------------------------
    // IAnimalState
    // ----------------------------------------------------------
    public override void Enter()
    {
        _elapsed  = 0f;
        _startPos = Owner.transform.position;

        // Flee directly away from the player
        Vector2 fleeDir = ((Vector2)Owner.transform.position
                         - (Vector2)Player.Instance.transform.position).normalized;

        // Fallback: flee in a random direction if player is on top of the chicken
        if (fleeDir == Vector2.zero)
            fleeDir = Random.insideUnitCircle.normalized;

        _targetPos = Owner.ClampToBounds(_startPos + fleeDir * Owner.ChickenData.fleeDistance);
        Owner.FaceDirection(fleeDir);
    }

    public override void Tick()
    {
        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / Owner.ChickenData.fleeDuration);

        Owner.transform.position = Vector2.Lerp(_startPos, _targetPos, t);

        if (t >= 1f)
            Owner.StateMachine.ChangeState(Owner.IdleState);
    }
}
