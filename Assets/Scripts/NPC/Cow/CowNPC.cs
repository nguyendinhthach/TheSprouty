// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/CowNPC.cs
// Concrete cow NPC shared by all color variants.
// Visual differences handled by CowAnimator subclasses.
// ──────────────────────────────────────────────
using UnityEngine;

public class CowNPC : BaseAnimalNPC, IInteractable, IUsable
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

    /// <summary>Trigger HappyState when the player successfully feeds this cow.</summary>
    protected override void OnFedSuccessfully()
    {
        StateMachine.ChangeState(HappyState);
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
    // IUsable
    // ----------------------------------------------------------

    /// <summary>
    /// Called by Player.PerformToolAction when ToolType.None and indicator is on this cow.
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
}
