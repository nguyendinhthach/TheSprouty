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
        IAnimalState current = chicken.StateMachine.CurrentState;

        // State changed → cancel sub-action, play new state animation immediately
        if (current != _previousState)
        {
            _playingSubAction = false;
            _previousState    = current;
            PlayAnimationForState(current);
            return;
        }

        // Sub-action: detect khi Eat xong rồi force re-trigger idle
        if (_playingSubAction)
        {
            AnimatorStateInfo info = _animator.GetCurrentAnimatorStateInfo(0);
            if (info.IsName(ANIM_EAT) && !info.loop && info.normalizedTime >= 0.95f)
            {
                _playingSubAction = false;
                _previousState    = null; // force re-trigger next frame
            }
        }
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
        string anim = state switch
        {
            ChickenIdleState        idle => idle.UseBlinkVariant ? ANIM_BLINK_IDLE : ANIM_LOOK_AROUND,
            ChickenWanderState      _    => ANIM_WANDER,
            ChickenWalkToNestState  _    => ANIM_WANDER,
            ChickenLayDownGrassState _   => ANIM_LAY_DOWN_GRASS,
            ChickenSleepGrassState  _    => ANIM_SLEEP_GRASS,
            ChickenGetUpGrassState  _    => ANIM_GET_UP_GRASS,
            ChickenJumpIntoNestState _   => ANIM_JUMP_INTO_NEST,
            ChickenSleepInNestState _    => ANIM_SLEEP_IN_NEST,
            ChickenJumpOutNestState _    => ANIM_JUMP_OUT_NEST,
            ChickenIncubatingState  _    => ANIM_INCUBATING,
            ChickenFleeState        _    => ANIM_FLEE,
            ChickenHappyState       _    => ANIM_HAPPY,
            _                            => null
        };

        if (anim != null)
            _animator.Play(anim, -1, 0f);
    }
}
