using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Tree : ResourceNode
{
    private const string ANIM_HIT = "Hit";
    private const string ANIM_FALL = "Fall";

    [Tooltip("Thời gian khớp với độ dài clip Tree_FallDown")]
    [SerializeField] private float fallDuration = 1f;

    protected virtual Animator TreeAnimator => null;

    protected override void OnHit(ToolSO playerTool)
    {
        TreeAnimator?.SetTrigger(ANIM_HIT);
    }

    protected override void OnDestroyed()
    {
        TreeAnimator?.SetTrigger(ANIM_FALL);
    }

    // Override DestroyNode timing — chờ anim xong rồi mới Destroy
    protected override float DestroyDelay => fallDuration;
}
