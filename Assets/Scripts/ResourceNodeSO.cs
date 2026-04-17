using UnityEngine;

[CreateAssetMenu(fileName = "New Resource Node", menuName = "TheSprouty/Environment/Resource Node")]
public class ResourceNodeSO : ScriptableObject
{
    [Header("Node Data")]
    public string nodeName;
    public int maxHealth = 100;
    public Transform prefab;
}
