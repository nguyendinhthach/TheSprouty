// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenNPC.cs
// Concrete chicken NPC. Wires all states, handles
// IInteractable and player-tool interaction logic.
// ──────────────────────────────────────────────
using System;
using UnityEngine;

public class ChickenNPC : BaseAnimalNPC, IInteractable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private ChickenDataSO chickenData;

    [Tooltip("BoxCollider2D defining the area the chicken can wander in.")]
    [SerializeField] private BoxCollider2D wanderBounds;

    [Tooltip("Assign the nest zone Transform so the chicken knows where its nest is.")]
    [SerializeField] private Transform nestZoneTransform;

    // ----------------------------------------------------------
    // States  (public so states can cross-reference for transitions)
    // ----------------------------------------------------------
    public ChickenIdleState         IdleState         { get; private set; }
    public ChickenWanderState       WanderState       { get; private set; }
    public ChickenLayDownGrassState LayDownGrassState { get; private set; }
    public ChickenSleepGrassState   SleepGrassState   { get; private set; }
    public ChickenGetUpGrassState   GetUpGrassState   { get; private set; }
    public ChickenJumpIntoNestState JumpIntoNestState { get; private set; }
    public ChickenSleepInNestState  SleepInNestState  { get; private set; }
    public ChickenIncubatingState   IncubatingState   { get; private set; }
    public ChickenJumpOutNestState  JumpOutNestState  { get; private set; }
    public ChickenWalkToNestState   WalkToNestState   { get; private set; }
    public ChickenFleeState         FleeState         { get; private set; }
    public ChickenHappyState        HappyState        { get; private set; }

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public ChickenDataSO ChickenData => chickenData;
    public override AnimalNPCSO AnimalData => chickenData;

    /// <summary>World position of the nest entry point.</summary>
    public Vector2 NestZonePosition => nestZoneTransform != null
        ? (Vector2)nestZoneTransform.position
        : (Vector2)transform.position;


    /// <summary>True when the player indicator is currently hovering over this chicken.</summary>
    public bool IsTargetedByIndicator { get; private set; }

    /// <summary>True when player is inside the DetectZone trigger.</summary>
    public bool IsPlayerInRange { get; private set; }

    /// <summary>Called by ChickenDetectZone when player enters/exits the trigger.</summary>
    public void SetPlayerInRange(bool value) => IsPlayerInRange = value;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private ChickenAnimator _chickenAnimator;
    private float           _happyCooldownTimer;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        _chickenAnimator = GetComponentInChildren<ChickenAnimator>();
    }

    protected override void Update()
    {
        base.Update();
        if (_happyCooldownTimer > 0f)
            _happyCooldownTimer -= Time.deltaTime;
    }

    protected override void Start()
    {
        base.Start(); // InitializeStates() được gọi trong base.Start()
        // Subscribe sau khi tất cả Awake() đã chạy → Player.Instance chắc chắn tồn tại
        if (Player.Instance != null)
            Player.Instance.OnToolUsed += OnPlayerUsedTool;
        else
            Debug.LogWarning("[ChickenNPC] Player.Instance is null in Start — tool events won't work.");
    }

    private void OnDestroy()
    {
        if (Player.Instance != null)
            Player.Instance.OnToolUsed -= OnPlayerUsedTool;
    }

    // ----------------------------------------------------------
    // Protected hooks
    // ----------------------------------------------------------
    protected override void InitializeStates()
    {
        IdleState         = new ChickenIdleState(this);
        WanderState       = new ChickenWanderState(this);
        LayDownGrassState = new ChickenLayDownGrassState(this);
        SleepGrassState   = new ChickenSleepGrassState(this);
        GetUpGrassState   = new ChickenGetUpGrassState(this);
        JumpIntoNestState = new ChickenJumpIntoNestState(this);
        SleepInNestState  = new ChickenSleepInNestState(this);
        IncubatingState   = new ChickenIncubatingState(this);
        JumpOutNestState  = new ChickenJumpOutNestState(this);
        WalkToNestState   = new ChickenWalkToNestState(this);
        FleeState         = new ChickenFleeState(this);
        HappyState        = new ChickenHappyState(this);

        StateMachine.Initialize(IdleState);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Clamp a position to stay inside the wander bounds.</summary>
    public Vector2 ClampToBounds(Vector2 pos)
    {
        if (wanderBounds == null) return pos;
        Bounds b = wanderBounds.bounds;
        return new Vector2(
            Mathf.Clamp(pos.x, b.min.x, b.max.x),
            Mathf.Clamp(pos.y, b.min.y, b.max.y));
    }

    /// <summary>Returns a random point inside the wander bounds.</summary>
    public Vector2 GetRandomWanderPoint()
    {
        if (wanderBounds != null)
        {
            Bounds b = wanderBounds.bounds;
            return new Vector2(
                UnityEngine.Random.Range(b.min.x, b.max.x),
                UnityEngine.Random.Range(b.min.y, b.max.y));
        }
        // Fallback: random point within wander radius
        return (Vector2)transform.position
             + UnityEngine.Random.insideUnitCircle * chickenData.wanderRadiusMax;
    }

    /// <summary>Trigger the Eat micro-animation without changing FSM state.</summary>
    public void PlayEatSubAction() => _chickenAnimator?.PlayEatSubAction();

    /// <summary>True when ChickenAnimator is currently playing the Eat sub-action.</summary>
    public bool IsPlayingEatSubAction => _chickenAnimator != null && _chickenAnimator.IsPlayingSubAction;

    // ----------------------------------------------------------
    // IInteractable
    // ----------------------------------------------------------
    public void OnIndicatorEnter() => IsTargetedByIndicator = true;
    public void OnIndicatorExit()  => IsTargetedByIndicator = false;

    // ----------------------------------------------------------
    // Private event handlers
    // ----------------------------------------------------------
    private void OnPlayerUsedTool(object sender, Player.ToolUsedEventArgs e)
    {
        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);

        // Tool equipped + player trong flee radius → flee
        if (e.ToolType != ToolType.None && dist <= chickenData.fleeRadius)
        {
            StateMachine.ChangeState(FleeState);
            return;
        }

        // Tay không + player trong flee radius + hết cooldown → happy
        if (e.ToolType == ToolType.None && dist <= chickenData.fleeRadius
            && _happyCooldownTimer <= 0f)
        {
            _happyCooldownTimer = chickenData.happyCooldown;
            StateMachine.ChangeState(HappyState);
        }
    }
}