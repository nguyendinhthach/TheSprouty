using System.Collections;
using UnityEngine;

[RequireComponent(typeof(ItemPickup))]
public class ItemBounceObject : MonoBehaviour
{
    [Header("Bounce Settings")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float jumpHeight = 1.2f;

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------
    public void StartBounce(Vector3 startPos, Vector3 targetPos)
    {
        StartCoroutine(BounceRoutine(startPos, targetPos));
    }

    // ----------------------------------------------------------
    // Private coroutine
    // ----------------------------------------------------------
    private IEnumerator BounceRoutine(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            Vector3 pos = Vector3.Lerp(start, end, t);
            pos.y += Mathf.Sin(t * Mathf.PI) * jumpHeight;

            transform.position = pos;
            yield return null;
        }

        transform.position = end;
        GetComponent<ItemPickup>().CanBePickedUp = true;
    }
}
