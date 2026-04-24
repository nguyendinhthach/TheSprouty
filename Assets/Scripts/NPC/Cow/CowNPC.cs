// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/CowNPC.cs
// Concrete cow NPC shared by all color variants.
// Visual differences handled by CowAnimator subclasses.
// ──────────────────────────────────────────────
using UnityEngine;

public class CowNPC : BaseAnimalNPC, IInteractable
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private CowDataSO cowData;

    // ----------------------------------------------------------
    // States
    // ----------------------------------------------------------
    public CowIdleState    IdleState    { get; private set; }
    public CowWanderState  WanderState  { get; private set; }
    public CowLayDownState LayDownState { get; private set; }
    public CowSitIdleState SitIdleState { get; private set; }
    public CowSleepState   SleepState   { get; private set; }
    public CowGetUpState   GetUpState   { get; private set; }
    public CowHappyState   HappyState   { get; private set; }

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public CowDataSO CowData => cowData;
    public override AnimalNPCSO AnimalData => cowData;

    /// <summary>True when the player indicator is hovering over this cow.</summary>
    public bool IsTargetedByIndicator { get; private set; }

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private CowAnimator _cowAnimator;
    private float       _happyCooldownTimer;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    protected override void Awake()
    {
        base.Awake();
        _cowAnimator = GetComponentInChildren<CowAnimator>();
    }

    protected override void Start()
    {
        base.Start();
        if (Player.Instance != null)
            Player.Instance.OnToolUsed += OnPlayerUsedTool;
        else
            Debug.LogWarning("[CowNPC] Player.Instance is null in Start.");
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
        IdleState    = new CowIdleState(this);
        WanderState  = new CowWanderState(this);
        LayDownState = new CowLayDownState(this);
        SitIdleState = new CowSitIdleState(this);
        SleepState   = new CowSleepState(this);
        GetUpState   = new CowGetUpState(this);
        HappyState   = new CowHappyState(this);

        StateMachine.Initialize(IdleState);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Play Sniff (true) or Graze (false) as a micro sub-action.</summary>
    public void PlaySubAction(bool sniff) => _cowAnimator?.PlaySubAction(sniff);

    /// <summary>True when a sub-action is currently playing.</summary>
    public bool IsPlayingSubAction => _cowAnimator != null && _cowAnimator.IsPlayingSubAction;

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
        if (e.ToolType != ToolType.None) return;

        float dist = Vector2.Distance(transform.position, Player.Instance.transform.position);
        if (dist <= cowData.interactRadius && _happyCooldownTimer <= 0f)
        {
            _happyCooldownTimer = cowData.happyCooldown;
            StateMachine.ChangeState(HappyState);
        }
    }
}
