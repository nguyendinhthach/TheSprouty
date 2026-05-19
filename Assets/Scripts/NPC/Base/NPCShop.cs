// ──────────────────────────────────────────────
// TheSprouty | NPC/Base/NPCShop.cs
// Shop NPC: detects player proximity and opens DialoguePanel on E press.
// ──────────────────────────────────────────────
using UnityEngine;

public class NPCShop : BaseTriggerZone
{
    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [SerializeField] private GameObject interactIndicator;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private bool _playerInRange;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Update()
    {
        if (!_playerInRange) return;

        interactIndicator.SetActive(!Player.Instance.IsInDialogue);

        if (!Input.GetKeyDown(KeyCode.E)) return;
        if (!Player.Instance.IsInDialogue)
            DialoguePanelUI.Instance.Open();
    }

    // ----------------------------------------------------------
    // Protected hooks
    // ----------------------------------------------------------
    protected override void OnPlayerEnter()
    {
        _playerInRange = true;
        interactIndicator.SetActive(true);
    }

    protected override void OnPlayerExit()
    {
        _playerInRange = false;
        interactIndicator.SetActive(false);
    }

}
