// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/States/ChickenFleeState.cs
// Chicken flees away from the player by lerping to a
// calculated flee position over the clip's duration.
// Flee target is clamped to NavMesh to stay within bounds.
// ──────────────────────────────────────────────
using UnityEngine;
using UnityEngine.AI;

public class ChickenFleeState : BaseAnimalState<ChickenNPC>
{
    private float   _elapsed;
    private Vector2 _startPos;
    private Vector2 _targetPos;

    public ChickenFleeState(ChickenNPC owner) : base(owner) { }

    public override void Enter()
    {
        // Giải phóng tổ nếu gà bị flee trong khi đang ở nest
        Owner.VacateNest();

        Owner.StopAgent();
        _elapsed  = 0f;
        _startPos = Owner.transform.position;

        Vector2 fleeDir = ((Vector2)Owner.transform.position
                         - (Vector2)Player.Instance.transform.position).normalized;

        if (fleeDir == Vector2.zero)
            fleeDir = Random.insideUnitCircle.normalized;

        Vector3 rawTarget = (Vector3)(_startPos + fleeDir * Owner.ChickenData.fleeDistance);

        // Clamp flee target vào trong NavMesh để không bay ra ngoài fence
        NavMeshHit hit;
        if (UnityEngine.AI.NavMesh.SamplePosition(rawTarget, out hit, Owner.ChickenData.fleeDistance, UnityEngine.AI.NavMesh.AllAreas))
            _targetPos = hit.position;
        else
            _targetPos = _startPos; // không tìm được điểm hợp lệ → đứng yên

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

    public override void Exit()
    {
        // Warp NavMeshAgent đến vị trí mới sau flee
        // để nó không snap về vị trí cũ khi resume
        if (Owner.Agent.isOnNavMesh)
            Owner.Agent.Warp(Owner.transform.position);
        Owner.ResumeAgent();
    }
}
