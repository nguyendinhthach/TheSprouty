// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenNPC.cs
// Concrete chicken NPC. Wires all states, handles
// IInteractable (indicator hover) and IUsable (feeding interaction).
// ──────────────────────────────────────────────
using UnityEngine;

public class ChickenNPC : BaseAnimalNPC, IInteractable, IUsable
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
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

    /// <summary>Trigger HappyState when the player successfully feeds this chicken.</summary>
    protected override void OnFedSuccessfully()
    {
        StateMachine.ChangeState(HappyState);
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
    // IUsable
    // ----------------------------------------------------------

    /// <summary>
    /// Called by Player.PerformToolAction when ToolType.None and indicator is on this chicken.
    /// Checks inventory for feedItem, consumes it, and triggers HappyState via OnFedSuccessfully().
    /// </summary>
    public void Use()
    {
        FeedResult result = TryFeed();

        switch (result)
        {
            case FeedResult.Success:
                // OnFedSuccessfully() đã được gọi bên trong TryFeed()
                break;

            case FeedResult.AlreadyFed:
                NotificationManager.Instance?.ShowMessage("Already fed today!");
                break;

            case FeedResult.InsufficientFeed:
                string itemName = AnimalData?.feedItem?.itemName ?? "feed";
                NotificationManager.Instance?.ShowMessage($"Need {itemName}!");
                break;

            case FeedResult.NoFeedConfigured:
                // Silent fail — feedItem chưa được config trong Inspector
                break;
        }
    }

    // ----------------------------------------------------------
    // Private event handlers
    // ----------------------------------------------------------

    /// <summary>
    /// Listens for tool usage events to trigger Flee when player uses a tool nearby.
    /// Happy/feeding logic is handled by IUsable.Use() — not here.
    /// </summary>
    private void OnPlayerUsedTool(object sender, Player.ToolUsedEventArgs e)
    {
        // Chỉ flee khi player dùng tool (không phải tay không)
        if (e.ToolType == ToolType.None) return;

        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (dist <= chickenData.fleeRadius)
            StateMachine.ChangeState(FleeState);
    }
}
