using System.Collections.Generic;
using UnityEngine;

public class Rock : ResourceNode
{
    [Header("Drop On Hit")]
    [SerializeField] private List<DropEntry> rockDropOnHit;

    [Header("Rock FX")]
    [SerializeField] private ParticleSystem hitParticles;

    protected override void OnHit(ToolSO playerTool)
    {
        hitParticles?.Play();
    }

    protected override void OnHitDrop()
    {
        if (rockDropOnHit == null || rockDropOnHit.Count == 0) return;

        foreach (DropEntry entry in rockDropOnHit)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            SpawnDrops(entry, amount);
        }
    }
}
