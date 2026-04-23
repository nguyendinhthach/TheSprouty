// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenNavMeshTest.cs
// Script test NavMesh đơn giản — độc lập với ChickenNPC.
// Gắn lên GameObject có NavMeshAgent để kiểm tra.
// ──────────────────────────────────────────────
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ChickenNavMeshTest : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("Wander Settings")]
    [SerializeField] private float wanderRadius = 3f;
    [SerializeField] private float idleTime     = 2f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private NavMeshAgent _agent;
    private float        _idleTimer;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.updateRotation = false;
        _agent.updateUpAxis   = false;
    }

    private void Start()
    {
        // ── Debug: kiểm tra agent có trên NavMesh không ──
        Debug.Log($"[NavMeshTest] isOnNavMesh: {_agent.isOnNavMesh} | pos: {transform.position}");

        if (!_agent.isOnNavMesh)
        {
            Debug.LogWarning("[NavMeshTest] Agent KHÔNG nằm trên NavMesh! Kiểm tra Y position và bake lại.");
            return;
        }

        SetNewDestination();
    }

    private void Update()
    {
        if (!_agent.isOnNavMesh) return;

        if (HasArrived())
        {
            _idleTimer -= Time.deltaTime;
            if (_idleTimer <= 0f)
                SetNewDestination();
        }
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void SetNewDestination()
    {
        Vector3 randomDir = (Vector3)(Random.insideUnitCircle * wanderRadius) + transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDir, out hit, wanderRadius, NavMesh.AllAreas))
        {
            _agent.SetDestination(hit.position);
            Debug.Log($"[NavMeshTest] New destination: {hit.position}");
        }
        else
        {
            Debug.LogWarning("[NavMeshTest] SamplePosition không tìm được điểm hợp lệ!");
        }

        _idleTimer = idleTime;
    }

    private bool HasArrived()
    {
        return !_agent.pathPending
            && _agent.remainingDistance <= _agent.stoppingDistance;
    }
}
