// ──────────────────────────────────────────────
// TheSprouty | Scripts/DebrisObject.cs
// Visual-only debris piece. Bounces outward via arc then self-destructs.
// No pickup logic — stone drops are handled by Boulder's drop system.
// ──────────────────────────────────────────────
using System.Collections;
using UnityEngine;

public class DebrisObject : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Tooltip("Height of the arc at the peak of the bounce.")]
    [SerializeField] private float arcHeight = 0.6f;

    [Tooltip("How long the bounce arc takes to complete.")]
    [SerializeField] private float bounceDuration = 0.3f;

    [Tooltip("How long the debris stays on the ground before disappearing.")]
    [SerializeField] private float lingerDuration = 0.4f;

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>
    /// Kicks off the debris arc from origin to target, then lingers and self-destructs.
    /// Call this immediately after Instantiate.
    /// </summary>
    public void Launch(Vector3 origin, Vector3 target)
    {
        StartCoroutine(BounceRoutine(origin, target));
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------

    private IEnumerator BounceRoutine(Vector3 origin, Vector3 target)
    {
        float elapsed = 0f;

        while (elapsed < bounceDuration)
        {
            float t = elapsed / bounceDuration;

            // Lerp horizontally from origin to target
            Vector3 horizontal = Vector3.Lerp(origin, target, t);

            // Parabolic arc on Y axis: peaks at t=0.5, returns to 0 at t=1
            float arc = arcHeight * Mathf.Sin(t * Mathf.PI);

            transform.position = horizontal + new Vector3(0f, arc, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;

        // Linger briefly on the ground then disappear
        yield return new WaitForSeconds(lingerDuration);
        Destroy(gameObject);
    }
}
