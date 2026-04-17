using System.Diagnostics;
using System.Linq;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Tree : ResourceNode
{
    [Header("Tree FX")]
    [SerializeField] private Animator treeAnimator;

    private const string ANIM_HIT = "Hit";
    private const string ANIM_FALL = "Fall";

    protected override void OnHit(ToolSO playerTool)
    {
        //treeAnimator?.SetTrigger(ANIM_HIT);
        // Future: play axe-chop sound here
    }

    protected override void OnDestroyed()
    {
        //treeAnimator?.SetTrigger(ANIM_FALL);
        // Future: spawn stump, play falling sound, screen shake
    }
}
