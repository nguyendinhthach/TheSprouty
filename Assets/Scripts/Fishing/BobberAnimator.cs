// ──────────────────────────────────────────────
// TheSprouty | Fishing/BobberAnimator.cs
// Controls Bobber animator states.
// ──────────────────────────────────────────────
using UnityEngine;

public class BobberAnimator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Animator parameter keys
    // ----------------------------------------------------------
    private const string TRIGGER_BITE  = "Bobber_Bite";
    private const string TRIGGER_RESET = "Bobber_Reset";

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Animator _animator;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Plays Bitten animation — called when fish bites.</summary>
    public void PlayBitten() => _animator.SetTrigger(TRIGGER_BITE);

    /// <summary>Resets to Idle — called when nibble window expires without LMB.</summary>
    public void ResetToIdle() => _animator.SetTrigger(TRIGGER_RESET);
}
