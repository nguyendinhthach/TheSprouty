// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/BaseAnimalNPC.cs
// Abstract root class for all animal NPCs.
// Owns the FSM, NavMeshAgent, and common utilities.
// ──────────────────────────────────────────────
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class BaseAnimalNPC : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField, HideInInspector] protected AnimalNPCSO animalData;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public AnimalStateMachine StateMachine { get; private set; }
    public NavMeshAgent        Agent       { get; private set; }
    public virtual AnimalNPCSO AnimalData  => animalData;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private SpriteRenderer _spriteRenderer;
    private Animator       _animator;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    protected virtual void Awake()
    {
        Agent           = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator       = GetComponentInChildren<Animator>();
        StateMachine    = new AnimalStateMachine();

        // Configure NavMeshAgent for 2D (NavMeshPlus)
        Agent.updateRotation = false;
        Agent.updateUpAxis   = false;
    }

    protected virtual void Start()
    {
        StartCoroutine(InitializeAgentRoutine());
    }

    protected virtual void Update()
    {
        StateMachine.Tick();
        FaceMovementDirection();
    }

    // ----------------------------------------------------------
    // Abstract
    // ----------------------------------------------------------
    /// <summary>Create all states and call StateMachine.Initialize(startState).</summary>
    protected abstract void InitializeStates();

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Flip the sprite to face the given direction (x-axis only).</summary>
    public void FaceDirection(Vector2 dir)
    {
        if (dir.x == 0f || _spriteRenderer == null) return;
        _spriteRenderer.flipX = dir.x < 0f;
    }

    /// <summary>
    /// Sample a random valid NavMesh position within the wander radius.
    /// Returns false if no valid point is found.
    /// </summary>
    public bool TryGetRandomWanderPoint(out Vector3 result)
    {
        if (AnimalData == null) { result = transform.position; return false; }

        float   radius    = Random.Range(AnimalData.wanderRadiusMin, AnimalData.wanderRadiusMax);
        Vector3 randomDir = (Vector3)(Random.insideUnitCircle * radius) + transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDir, out hit, radius, NavMesh.AllAreas))
        {
            result = hit.position;
            return true;
        }

        result = transform.position;
        return false;
    }

    /// <summary>Safely stop the NavMeshAgent — no-op if not on a NavMesh.</summary>
    public void StopAgent()
    {
        if (!Agent.isOnNavMesh) return;
        Agent.SetDestination(transform.position); // giữ avoidance, không di chuyển
        Agent.speed = 0f;
    }

    public void ResumeAgent()
    {
        if (!Agent.isOnNavMesh) return;
        Agent.speed = AnimalData != null ? AnimalData.moveSpeed : 2f;
    }

    /// <summary>
    /// Move toward target directly via transform (used for Flee, no pathfinding).
    /// Returns true when arrived.
    /// </summary>
    public bool MoveToward(Vector2 target)
    {
        if (AnimalData == null) return true;
        Vector2 current = transform.position;
        Vector2 next    = Vector2.MoveTowards(current, target, AnimalData.moveSpeed * Time.deltaTime);
        transform.position = next;
        FaceDirection(target - current);
        return Vector2.Distance(next, target) < 0.05f;
    }

    /// <summary>
    /// Returns true when the currently playing animation clip has finished.
    /// Used by one-shot states to auto-transition.
    /// </summary>
    public bool IsCurrentAnimationDone()
    {
        if (_animator == null) return true;
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        return !info.loop && info.normalizedTime >= 0.95f;
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private System.Collections.IEnumerator InitializeAgentRoutine()
    {
        Agent.enabled = false;
        Agent.enabled = true;
        yield return null;
        Agent.Warp(transform.position);
        yield return null;
        InitializeStates();
    }

    private void FaceMovementDirection()
    {
        if (Agent.isOnNavMesh && Agent.velocity.sqrMagnitude > 0.01f)
            FaceDirection(Agent.velocity);
    }
}
