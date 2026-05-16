// ──────────────────────────────────────────────
// TheSprouty | UI/ButtonHoverTextColor.cs
// Changes TMP text color on pointer enter/exit.
// Outline color is unaffected (controlled by font material).
// ──────────────────────────────────────────────
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonHoverTextColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private TMP_Text label;
    [SerializeField] private Color normalColor = Color.black;
    [SerializeField] private Color hoverColor  = new Color(0.2f, 0.55f, 1f);

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Start()
    {
        label.color = normalColor;
    }

    // ----------------------------------------------------------
    // Pointer events
    // ----------------------------------------------------------
    public void OnPointerEnter(PointerEventData _) => label.color = hoverColor;
    public void OnPointerExit(PointerEventData _)  => label.color = normalColor;
}
