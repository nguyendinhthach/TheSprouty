// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/BaseAnimalNPC.cs
// Abstract root class for all animal NPCs.
// Owns the FSM and common movement utilities.
// ──────────────────────────────────────────────
using UnityEngine;

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
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _animator       = GetComponentInChildren<Animator>();
        StateMachine    = new AnimalStateMachine();
    }

    protected virtual void Start()
    {
        InitializeStates();
    }

    protected virtual void Update()
    {
        StateMachine.Tick();
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

    /// <summary>Move toward target at moveSpeed. Returns true when arrived.</summary>
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
    /// Used by one-shot states (LayDown, JumpIntoNest, etc.) to auto-transition.
    /// </summary>
    public bool IsCurrentAnimationDone()
    {
        if (_animator == null) return true;
        AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
        return !info.loop && info.normalizedTime >= 0.95f;
    }
}
