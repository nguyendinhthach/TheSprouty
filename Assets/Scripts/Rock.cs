using UnityEngine;

public class Rock : ResourceNode
{
    [Header("Rock FX")]
    [SerializeField] private ParticleSystem hitParticles;

    protected override void OnHit(ToolSO playerTool)
    {
        hitParticles?.Play();
    }
}
