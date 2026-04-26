using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ItemPickup : MonoBehaviour
{
    [Header("Magnet Settings")]
    [SerializeField] private float attractSpeed = 8f;
    [SerializeField] private float destroyDistance = 0.2f;

    // ENCAPSULATION: only ItemBounceObject (same assembly) can
    // enable pickup; external code reads it but cannot set it.
    public bool CanBePickedUp { get; set; } = false;

    private bool _isMagnetized;
    private Transform _playerTransform;
    private ICollectable _collectable;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _collectable = GetComponent<ICollectable>();
    }

    private void Update()
    {
        if (!_isMagnetized || _playerTransform == null) return;

        MoveTowardPlayer();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!CanBePickedUp) return;
        if (_isMagnetized) return;
        if (!other.CompareTag("Magnet")) return;

        Magnetize(other.transform);
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void Magnetize(Transform target)
    {
        _isMagnetized = true;
        _playerTransform = target;
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            _playerTransform.position,
            attractSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, _playerTransform.position) < destroyDistance)
            Collect();
    }

    private void Collect()
    {
        _collectable?.OnCollected();
        Destroy(gameObject);
    }
}
