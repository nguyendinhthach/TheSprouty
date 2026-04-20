using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class DropEntry
{
    public ItemSO item;
    public int minAmount = 1;
    public int maxAmount = 1;
    [Range(0f, 100f)]
    public float dropChance = 100f;
}

// ============================================================
// HarvestRecipeSO — defines valid tools and loot table for harvesting.
// Node identity (HP, name, prefab) is now held by ResourceNodeSO
// on the ResourceNode component directly — not here.
// ============================================================
[CreateAssetMenu(fileName = "New Harvest Recipe", menuName = "TheSprouty/Environment/Harvest Recipe")]
public class HarvestRecipeSO : ScriptableObject
{
    [Header("Input")]
    [Tooltip("Which tools can harvest this node? Leave empty = any tool (including bare hands).")]
    public ToolSO[] validTools;

    [Header("Output")]
    public List<DropEntry> dropList;



    public bool IsValidTool(ToolSO toolToCheck)
    {
        if (validTools == null || validTools.Length == 0)
            return true;                      // Không hạn chế tool nào → tay không cũng được


        foreach (ToolSO tool in validTools)
        {
            if (tool == toolToCheck) return true;
        }

        return false;   // Player has a tool but it's the wrong type
    }
}
