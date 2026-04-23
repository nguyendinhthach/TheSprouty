// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenNPC.cs
// Concrete chicken NPC. Wires all states, handles
// IInteractable and player-tool interaction logic.
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenNPC : BaseAnimalNPC, IInteractable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private ChickenDataSO chickenData;

    [Tooltip("Tất cả các tổ trong chuồng gà này.")]
    [SerializeField] private Nest[] nests;

    // ----------------------------------------------------------
    // States  (public so states can cross-reference for transitions)
    // ----------------------------------------------------------
    public ChickenIdleState         IdleState         { get; private set; }
    public ChickenWanderState       WanderState       { get; private set; }
    public ChickenLayDownGrassState LayDownGrassState { get; private set; }
    public ChickenSleepGrassState   SleepGrassState   { get; private set; }
    public ChickenGetUpGrassState   GetUpGrassState   { get; private set; }
    public ChickenWalkToNestState   WalkToNestState   { get; private set; }
    public ChickenJumpIntoNestState JumpIntoNestState { get; private set; }
    public ChickenSleepInNestState  SleepInNestState  { get; private set; }
    public ChickenIncubatingState   IncubatingState   { get; private set; }
    public ChickenJumpOutNestState  JumpOutNestState  { get; private set; }
    public ChickenFleeState         FleeState         { get; private set; }
    public ChickenHappyState        HappyState        { get; private set; }

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public ChickenDataSO ChickenData => chickenData;
    public override AnimalNPCSO AnimalData => chickenData;

    /// <summary>Tổ mà gà hiện tại đang nhắm tới hoặc đang ở.</summary>
    private Nest _targetNest;

    /// <summary>Vị trí của tổ đang nhắm tới.</summary>
    public Vector2 NestZonePosition => _targetNest?.NestPoint != null
        ? (Vector2)_targetNest.NestPoint.position
        : (Vector2)transform.position;

    /// <summary>True nếu tổ đang nhắm tới vẫn còn trống.</summary>
    public bool IsTargetNestAvailable => _targetNest != null && !_targetNest.IsOccupied;

    /// <summary>Tìm và ghi nhớ 1 tổ trống. Trả về false nếu không có tổ nào trống.</summary>
    public bool SelectAvailableNest()
    {
        _targetNest = null;
        if (nests == null) return false;
        foreach (var n in nests)
        {
            if (n != null && !n.IsOccupied)
            {
                _targetNest = n;
                return true;
            }
        }
        return false;
    }

    /// <summary>True nếu có ít nhất 1 tổ trống (dùng để quyết định đi nest).</summary>
    public bool HasAnyNestAvailable
    {
        get
        {
            if (nests == null) return false;
            foreach (var n in nests)
                if (n != null && !n.IsOccupied) return true;
            return false;
        }
    }

    /// <summary>Chiếm tổ đang nhắm tới.</summary>
    public bool TryOccupyNest() => _targetNest != null && _targetNest.TryOccupy();

    /// <summary>Giải phóng tổ — gọi khi JumpOutNest xong.</summary>
    public void VacateNest()
    {
        _targetNest?.Vacate();
        _targetNest = null;
    }

    /// <summary>True when the player indicator is currently hovering over this chicken.</summary>
    public bool IsTargetedByIndicator { get; private set; }

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

    protected override void Start()
    {
        base.Start(); // InitializeStates() được gọi trong base coroutine
        if (Player.Instance != null)
            Player.Instance.OnToolUsed += OnPlayerUsedTool;
    }

    protected override void Update()
    {
        base.Update();
        if (_happyCooldownTimer > 0f)
            _happyCooldownTimer -= Time.deltaTime;
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
        WalkToNestState   = new ChickenWalkToNestState(this);
        JumpIntoNestState = new ChickenJumpIntoNestState(this);
        SleepInNestState  = new ChickenSleepInNestState(this);
        IncubatingState   = new ChickenIncubatingState(this);
        JumpOutNestState  = new ChickenJumpOutNestState(this);
        FleeState         = new ChickenFleeState(this);
        HappyState        = new ChickenHappyState(this);

        StateMachine.Initialize(IdleState);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

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