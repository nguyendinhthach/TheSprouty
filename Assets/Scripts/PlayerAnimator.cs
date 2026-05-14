using System.Collections;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Animator parameter keys  (ENCAPSULATION: private constants)
    // ----------------------------------------------------------
    private const string PARAM_HORIZONTAL = "Horizontal";
    private const string PARAM_VERTICAL = "Vertical";
    private const string PARAM_SPEED = "Speed";
    private const string PARAM_TOOL_TYPE = "ToolType";
    private const string PARAM_DO_ACTION = "DoAction";
    private const string PARAM_FISHING_REEL        = "Fishing_Reel";
    private const string PARAM_FISHING_CATCH       = "Fishing_Catch";
    private const string PARAM_FISHING_EXIT        = "Fishing_Exit";
    private const string PARAM_IS_FISHING_ON_WATER = "IsFishingOnWater";

    // Player body offset so direction is calculated from chest,
    // not from feet.
    private static readonly Vector3 BodyCenterOffset = new Vector3(0f, 0.5f, 0f);

    // ----------------------------------------------------------
    // Inspector fields
    // ----------------------------------------------------------
    [SerializeField] private Player player;
    [SerializeField] private WateringEffectController wateringEffect;
    [SerializeField] private FishingController fishingController;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Animator _animator;
    private PlayerIndicator _indicator;
    private Coroutine _unlockRoutine;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player.OnToolUsed += OnToolUsed;
        _indicator = FindAnyObjectByType<PlayerIndicator>();
    }

    private void Update()
    {
        UpdateLocomotionParameters();
    }

    // ----------------------------------------------------------
    // Animation events
    // ----------------------------------------------------------

    /// <summary>Called by the Animator at the exact frame the tool makes contact.</summary>
    public void AnimationEvent_PerformAction()
    {
        player.PerformToolAction();
    }

    /// <summary>
    /// Called by the Animator at the raise-can frame of the watering animation.
    /// Reads current facing direction and plays the matching spray effect.
    /// </summary>
    public void AnimationEvent_PlayWateringEffect()
    {
        if (wateringEffect == null) return;

        float h = _animator.GetFloat(PARAM_HORIZONTAL);
        float v = _animator.GetFloat(PARAM_VERTICAL);

        // Back direction — player sprite covers the effect, skip entirely
        if (Mathf.Abs(v) > Mathf.Abs(h) && v > 0f) return;

        WateringDirection dir;
        if (Mathf.Abs(h) > Mathf.Abs(v))
            dir = h > 0f ? WateringDirection.Right : WateringDirection.Left;
        else
            dir = WateringDirection.Front;

        wateringEffect.Play(dir);
    }

    /// <summary>Called by Animation Event on the last frame of Fishing_Cast clips.
    /// Signals FishingController that the cast animation has finished.</summary>
    public void AnimationEvent_CastComplete()
    {
        fishingController?.OnCastComplete();
    }

    /// <summary>Called by Animation Event on the last frame of Fishing_Happy clip.
    /// Signals FishingController that the full catch sequence has finished.</summary>
    public void AnimationEvent_FishingComplete()
    {
        fishingController?.OnFishingComplete();
    }

    /// <summary>Called by the Animator at the last frame of each tool animation.
    /// Unlocks player movement and indicator immediately via animation event.</summary>
    public void AnimationEvent_ActionComplete()
    {
        if (_unlockRoutine != null)
        {
            StopCoroutine(_unlockRoutine);
            _unlockRoutine = null;
        }
        player.UnlockAction();
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void UpdateLocomotionParameters()
    {
        Vector2 input = player.InputVector;

        if (player.IsMoving)
        {
            _animator.SetFloat(PARAM_HORIZONTAL, input.x);
            _animator.SetFloat(PARAM_VERTICAL, input.y);
        }

        _animator.SetFloat(PARAM_SPEED, input.sqrMagnitude);

        // Reset sprite flip khi không còn fishing VÀ transition đã hoàn tất.
        if (spriteRenderer != null && !fishingController.IsFishing && !_animator.IsInTransition(0))
            spriteRenderer.flipX = false;
    }

    private void OnToolUsed(object sender, Player.ToolUsedEventArgs e)
    {
        SetFacingTowardIndicator();
        ApplySideFlip(e.ToolType);

        if (e.ToolType == ToolType.FishingRod)
            fishingController?.OnCastStart();

        TriggerToolAnimation(e.ToolType);

        // FishingRod lifecycle is managed entirely by FishingController.
        // Skip WaitForActionCompleteRoutine to avoid unlocking prematurely.
        bool needsFallbackUnlock = e.ToolType != ToolType.None
                                && e.ToolType != ToolType.SeedBag
                                && e.ToolType != ToolType.FishingRod;

        if (needsFallbackUnlock)
        {
            if (_unlockRoutine != null) StopCoroutine(_unlockRoutine);
            _unlockRoutine = StartCoroutine(WaitForActionCompleteRoutine());
        }
    }

    private void SetFacingTowardIndicator()
    {
        if (_indicator == null) return;

        Vector3 origin = player.transform.position + BodyCenterOffset;
        Vector3 direction = (_indicator.transform.position - origin).normalized;

        // Snap to 4-directional facing: whichever axis is dominant wins.
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            direction.y = 0f;
        else
            direction.x = 0f;

        _animator.SetFloat(PARAM_HORIZONTAL, direction.x);
        _animator.SetFloat(PARAM_VERTICAL, direction.y);
    }

    /// <summary>
    /// Fallback: waits until the animator exits the current action state,
    /// then unlocks if AnimationEvent_ActionComplete was not called.
    /// </summary>
    private IEnumerator WaitForActionCompleteRoutine()
    {
        // Wait two frames for the trigger to be consumed and transition to begin
        yield return null;
        yield return null;

        int actionStateHash = _animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        // Wait until we leave the action state
        while (_animator.GetCurrentAnimatorStateInfo(0).fullPathHash == actionStateHash
               || _animator.IsInTransition(0))
        {
            yield return null;
        }

        _unlockRoutine = null;
        player.UnlockAction();
    }

    /// <summary>Triggers the Fishing_Reel animator parameter.</summary>
    public void TriggerFishingReel() => _animator.SetTrigger(PARAM_FISHING_REEL);

    /// <summary>Triggers the Fishing_Catch animator parameter after reel loops complete.</summary>
    public void TriggerFishingCatch() => _animator.SetTrigger(PARAM_FISHING_CATCH);

    /// <summary>Triggers the Fishing_Exit animator parameter (cancel or miss).</summary>
    public void TriggerFishingExit() => _animator.SetTrigger(PARAM_FISHING_EXIT);

    /// <summary>Sets IsFishingOnWater bool — controls Fishing_Cast transition destination.</summary>
    public void SetIsFishingOnWater(bool value) => _animator.SetBool(PARAM_IS_FISHING_ON_WATER, value);

    private void TriggerToolAnimation(ToolType toolType)
    {
        if (toolType == ToolType.None) return; // Không có animation để play

        _animator.SetInteger(PARAM_TOOL_TYPE, (int)toolType);
        _animator.SetTrigger(PARAM_DO_ACTION);
    }

    /// <summary>
    /// Flips the sprite horizontally for tools that only have a Side clip
    /// (no separate Left/Right clips). Other tools are unaffected.
    /// </summary>
    private void ApplySideFlip(ToolType toolType)
    {
        if (spriteRenderer == null) return;

        bool isSideOnlyTool = toolType == ToolType.FishingRod;
        if (!isSideOnlyTool) return;

        float h = _animator.GetFloat(PARAM_HORIZONTAL);
        // Side clip faces left by default → flip when facing right
        spriteRenderer.flipX = h > 0f;
    }
}
