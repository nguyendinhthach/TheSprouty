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

    // Player body offset so direction is calculated from chest,
    // not from feet.
    private static readonly Vector3 BodyCenterOffset = new Vector3(0f, 0.5f, 0f);

    // ----------------------------------------------------------
    // Inspector fields
    // ----------------------------------------------------------
    [SerializeField] private Player player;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Animator _animator;
    private PlayerIndicator _indicator;

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
    // Animation event — called by the Animator at the
    // exact frame the tool should make contact.
    // ----------------------------------------------------------
    public void AnimationEvent_PerformAction()
    {
        player.PerformToolAction();
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
    }

    private void OnToolUsed(object sender, Player.ToolUsedEventArgs e)
    {
        SetFacingTowardIndicator();
        TriggerToolAnimation(e.ToolType);
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

    private void TriggerToolAnimation(ToolType toolType)
    {
        if (toolType == ToolType.None) return; // Không có animation để play

        _animator.SetInteger(PARAM_TOOL_TYPE, (int)toolType);
        _animator.SetTrigger(PARAM_DO_ACTION);
    }
}
