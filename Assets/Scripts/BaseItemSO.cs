using UnityEngine;

public abstract class BaseItemSO : ScriptableObject
{
    [Header("Base Item Data")]
    public string itemName;
    public Sprite icon;
    public Transform prefab;
}
