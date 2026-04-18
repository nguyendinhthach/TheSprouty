using UnityEngine;

public class TimberTree : Tree
{
    [Header("Tree FX")]
    [SerializeField] private Animator treeAnimator;

    protected override Animator TreeAnimator => treeAnimator;
}
