using UnityEngine;

public abstract class BaseTriggerZone : MonoBehaviour
{
    // Template Method pattern — subclasses fill in the behavior.
    protected abstract void OnPlayerEnter();
    protected abstract void OnPlayerExit();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
            OnPlayerEnter();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.TryGetComponent<Player>(out _))
            OnPlayerExit();
    }
}
