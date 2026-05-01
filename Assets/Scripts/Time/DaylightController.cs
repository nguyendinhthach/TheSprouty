// ──────────────────────────────────────────────
// TheSprouty | Scripts/Time/DaylightController.cs
// Updates the scene's Global Light 2D color and intensity
// smoothly based on the current game hour from DayCycleManager.
// ──────────────────────────────────────────────
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DaylightController : MonoBehaviour
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------

    [Tooltip("The Global Light 2D in the scene used as ambient light.")]
    [SerializeField] private Light2D globalLight;

    [Tooltip("Same SO used by DayCycleManager — contains gradient and intensity curve.")]
    [SerializeField] private DayCycleSO dayCycleConfig;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------

    private void Update()
    {
        if (DayCycleManager.Instance == null) return;

        float normalizedTime = DayCycleManager.Instance.CurrentHour / 24f;

        globalLight.color     = dayCycleConfig.lightColorGradient.Evaluate(normalizedTime);
        globalLight.intensity = dayCycleConfig.lightIntensityCurve.Evaluate(normalizedTime);
    }
}
