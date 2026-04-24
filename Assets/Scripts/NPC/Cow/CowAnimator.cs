// ──────────────────────────────────────────────
// TheSprouty | NPC/Cow/CowAnimator.cs
// Abstract animator base for all cow variants.
// Subclasses provide color-specific animation names.
// ──────────────────────────────────────────────
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class CowAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Abstract animation name properties
    // Subclass overrides these to provide cow-specific clip names
    // ----------------------------------------------------------
    protected abstract string AnimBlinkIdle      { get; }
    protected abstract string AnimTailWagIdle    { get; }
    protected abstract string AnimWander         { get; }
    protected abstract string AnimLayDown        { get; }
    protected abstract string AnimSitBlinkIdle   { get; }
    protected abstract string AnimSitTailWagIdle { get; }
    protected abstract string AnimSleep          { get; }
    protected abstract string AnimGetUp          { get; }
    protected abstract string AnimSniff          { get; }
    protected abstract string AnimGraze          { get; }
    protected abstract string AnimHappy          { get; }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private CowNPC cow;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Animator     _animator;
    private IAnimalState _previousState;
    private bool         _playingSubAction;
    private bool         _subActionIsSniff;

    // ----------------------------------------------------------
    // Properties
    // ----------------------------------------------------------
    public bool IsPlayingSubAction => _playingSubAction;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        IAnimalState current = cow.StateMachine.CurrentState;

        // State changed → cancel sub-action, play new state animation immediately
        if (current != _previousState)
        {
            _playingSubAction = false;
            _previousState    = current;
            PlayAnimationForState(current);
            return;
        }

        // Sub-action: detect khi Sniff/Graze xong (normalizedTime >= 0.95)
        // rồi force play lại idle animation qua Play(-1, 0f).
        if (_playingSubAction)
        {
            AnimatorStateInfo info    = _animator.GetCurrentAnimatorStateInfo(0);
            string            subAnim = _subActionIsSniff ? AnimSniff : AnimGraze;
            if (info.IsName(subAnim) && !info.loop && info.normalizedTime >= 0.95f)
            {
                _playingSubAction = false;
                _previousState    = null; // force re-trigger PlayAnimationForState next frame
            }
        }
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Play Sniff (true) or Graze (false) as a micro sub-action.</summary>
    public void PlaySubAction(bool sniff)
    {
        if (_playingSubAction) return;
        _playingSubAction = true;
        _subActionIsSniff = sniff;
        _animator.Play(sniff ? AnimSniff : AnimGraze, -1, 0f);
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void PlayAnimationForState(IAnimalState state)
    {
        string anim = state switch
        {
            CowIdleState    idle => idle.UseBlinkVariant ? AnimBlinkIdle : AnimTailWagIdle,
            CowWanderState  _   => AnimWander,
            CowLayDownState _   => AnimLayDown,
            CowSitIdleState sit => sit.UseBlinkVariant ? AnimSitBlinkIdle : AnimSitTailWagIdle,
            CowSleepState   _   => AnimSleep,
            CowGetUpState   _   => AnimGetUp,
            CowHappyState   _   => AnimHappy,
            _                   => null
        };

        if (anim != null)
            _animator.Play(anim, -1, 0f);
    }
}
