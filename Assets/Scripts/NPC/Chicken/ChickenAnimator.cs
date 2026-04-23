// ──────────────────────────────────────────────
// TheSprouty | NPC/Chicken/ChickenAnimator.cs
// Drives the Animator from ChickenNPC's FSM state.
// Handles idle variant selection and eat sub-actions.
// ──────────────────────────────────────────────
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ChickenAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Animator state name constants
    // ----------------------------------------------------------
    private const string ANIM_BLINK_IDLE      = "ChickenDefault_BlinkIdle";
    private const string ANIM_LOOK_AROUND     = "ChickenDefault_LookAroundIdle";
    private const string ANIM_WANDER          = "ChickenDefault_Wander";
    private const string ANIM_LAY_DOWN_GRASS  = "ChickenDefault_LayDownGrass";
    private const string ANIM_SLEEP_GRASS     = "ChickenDefault_SleepGrass";
    private const string ANIM_GET_UP_GRASS    = "ChickenDefault_GetUpGrass";
    private const string ANIM_JUMP_INTO_NEST  = "ChickenDefault_JumpIntoNest";
    private const string ANIM_SLEEP_IN_NEST   = "ChickenDefault_SleepInNest";
    private const string ANIM_JUMP_OUT_NEST   = "ChickenDefault_JumpOutNest";
    private const string ANIM_INCUBATING      = "ChickenDefault_Incubating";
    private const string ANIM_EAT             = "ChickenDefault_Eat";
    private const string ANIM_FLEE            = "ChickenDefault_Flee";
    private const string ANIM_HAPPY           = "ChickenDefault_Happy";

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private ChickenNPC chicken;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Animator    _animator;
    private IAnimalState _previousState;
    private bool        _playingSubAction;

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
        // While a sub-action (Eat) is playing, wait for it to finish
        // then restore the idle animation.
        if (_playingSubAction)
        {
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (!info.IsName(ANIM_EAT) || (!info.loop && info.normalizedTime >= 1f))
            {
                _playingSubAction = false;
                PlayAnimationForState(chicken.StateMachine.CurrentState);
            }
            return;
        }

        // Drive animation from FSM state changes
        IAnimalState current = chicken.StateMachine.CurrentState;
        if (current == _previousState) return;

        _previousState = current;
        PlayAnimationForState(current);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>
    /// Play the Eat clip as a micro-animation without changing FSM state.
    /// The animator returns to the idle animation automatically when done.
    /// </summary>
    public void PlayEatSubAction()
    {
        if (_playingSubAction) return;
        _playingSubAction = true;
        _animator.Play(ANIM_EAT);
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void PlayAnimationForState(IAnimalState state)
    {
        switch (state)
        {
            case ChickenIdleState idle:
                _animator.Play(idle.UseBlinkVariant ? ANIM_BLINK_IDLE : ANIM_LOOK_AROUND);
                break;
            case ChickenWanderState _:
                _animator.Play(ANIM_WANDER);
                break;
            case ChickenWalkToNestState _:
                _animator.Play(ANIM_WANDER);
                break;
            case ChickenLayDownGrassState _:
                _animator.Play(ANIM_LAY_DOWN_GRASS);
                break;
            case ChickenSleepGrassState _:
                _animator.Play(ANIM_SLEEP_GRASS);
                break;
            case ChickenGetUpGrassState _:
                _animator.Play(ANIM_GET_UP_GRASS);
                break;
            case ChickenJumpIntoNestState _:
                _animator.Play(ANIM_JUMP_INTO_NEST);
                break;
            case ChickenSleepInNestState _:
                _animator.Play(ANIM_SLEEP_IN_NEST);
                break;
            case ChickenJumpOutNestState _:
                _animator.Play(ANIM_JUMP_OUT_NEST);
                break;
            case ChickenIncubatingState _:
                _animator.Play(ANIM_INCUBATING);
                break;
            case ChickenFleeState _:
                _animator.Play(ANIM_FLEE);
                break;
            case ChickenHappyState _:
                _animator.Play(ANIM_HAPPY);
                break;
        }
    }
}
