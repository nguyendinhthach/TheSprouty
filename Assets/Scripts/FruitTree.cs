using System.Collections.Generic;
using UnityEngine;

public class FruitTree : Tree
{
    [Header("Fruit Drop On Hit")]
    [SerializeField] private List<DropEntry> fruitDropOnHit;

    protected override void OnHitDrop()
    {
        if (fruitDropOnHit == null || fruitDropOnHit.Count == 0) return;

        foreach (DropEntry entry in fruitDropOnHit)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            SpawnDrops(entry, amount);
        }
    }
}