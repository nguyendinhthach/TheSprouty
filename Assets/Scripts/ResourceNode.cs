using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public abstract class ResourceNode : MonoBehaviour, IDamageable, IInteractable, IDroppable
{
    // ----------------------------------------------------------
    // Serialized fields  (ENCAPSULATION: inspector access only)
    // ----------------------------------------------------------
    [Header("Resource Config")]
    [SerializeField] private ResourceNodeSO resourceNodeSO;
    [field: SerializeField] protected HarvestRecipeSO harvestRecipeSO { get; set; }

    // ----------------------------------------------------------
    // Private state  (ENCAPSULATION: no direct external access)
    // ----------------------------------------------------------
    private int _currentHealth;

    // ----------------------------------------------------------
    // IDamageable  (ABSTRACTION via interface)
    // ----------------------------------------------------------
    public bool IsAlive => _currentHealth > 0;

    protected virtual void Start()
    {
        if (resourceNodeSO != null)
            _currentHealth = resourceNodeSO.maxHealth;
    }

    // ----------------------------------------------------------
    // IDamageable
    // ----------------------------------------------------------
    public virtual void TakeDamage(ToolSO playerTool)
    {
        if (harvestRecipeSO == null) return;
        if (!harvestRecipeSO.IsValidTool(playerTool)) return;

        _currentHealth -= playerTool.power;
        OnHit(playerTool);
        OnHitDrop();

        if (!IsAlive)
            DestroyNode();
    }

    // ----------------------------------------------------------
    // IDroppable
    // ----------------------------------------------------------
    public virtual void DropLoot()
    {
        if (harvestRecipeSO?.dropList == null || harvestRecipeSO.dropList.Count == 0) return;

        foreach (DropEntry entry in harvestRecipeSO.dropList)
        {
            if (Random.Range(0f, 100f) > entry.dropChance) continue;

            int amount = Random.Range(entry.minAmount, entry.maxAmount + 1);
            SpawnDrops(entry, amount);
        }
    }

    // ----------------------------------------------------------
    // IInteractable  
    // ----------------------------------------------------------
    public virtual void OnIndicatorEnter() { }
    public virtual void OnIndicatorExit() { }

    // ----------------------------------------------------------
    // Protected hooks
    // ----------------------------------------------------------

    /// <summary>Called once per hit, before destroy check. Override
    /// to add hit sounds, screen-shake, animations, etc.</summary>
    protected virtual void OnHit(ToolSO playerTool) { }

    protected virtual void OnHitDrop() { }


    /// <summary>Called when health reaches zero. Override to add
    /// custom death effects before the GameObject is destroyed.</summary>
    protected virtual void OnDestroyed() { }

    protected virtual float DestroyDelay => 0f;

    // ----------------------------------------------------------
    // Private implementation  (ENCAPSULATION)
    // ----------------------------------------------------------
    private void DestroyNode()
    {
        Player.Instance.ClearCurrentTarget();
        DropLoot();
        OnDestroyed();

        if (DestroyDelay > 0f)
            Invoke(nameof(DestroyGameObject), DestroyDelay);
        else
            DestroyGameObject();
    }

    // Helper — spawns a single drop entry N times with a bounce arc
    protected void SpawnDrops(DropEntry entry, int amount)
    {
        if (entry.item?.prefab == null) return;

        Vector3 origin = transform.position;

        for (int i = 0; i < amount; i++)
        {
            Vector3 scatter = new Vector3(
                Random.Range(-0.8f, 0.8f),
                Random.Range(-0.8f, 0.8f),
                0f
            );

            Transform spawnedItem = Instantiate(entry.item.prefab, origin, Quaternion.identity);

            if (spawnedItem.TryGetComponent<ItemBounceObject>(out var bounce))
                bounce.StartBounce(origin, origin + scatter);
        }
    }

    private void DestroyGameObject()
    {
        Destroy(gameObject);
    }
}