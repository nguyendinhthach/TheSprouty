using System;
using UnityEngine;

public class PlayerIndicator : MonoBehaviour
{
    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------
    public event EventHandler<SelectedResourceChangedEventArgs> OnSelectedResourceNodeChanged;

    public class SelectedResourceChangedEventArgs : EventArgs
    {
        public IInteractable SelectedResource;
    }

    // ----------------------------------------------------------
    // Inspector fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private Player player;

    [Header("Grid Settings")]
    [Tooltip("Snaps the origin to the player's center tile. Usually 0.1f.")]
    [SerializeField] private float playerGridOffset = 0.1f;
    [Tooltip("Centers the indicator in its tile. Usually 0.5f.")]
    [SerializeField] private float indicatorGridOffset = 0.5f;
    [SerializeField] private bool showOnNoTool = true;

    [Header("Detection")]
    [SerializeField] private LayerMask interactableLayerMask;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private IInteractable _currentInteractable;
    private SpriteRenderer _spriteRenderer;
    private Camera _mainCamera;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _mainCamera = Camera.main;
    }

    // LateUpdate keeps indicator smooth — runs after physics
    private void LateUpdate()
    {
        UpdateVisibility();
        if (!_spriteRenderer.enabled) return;

        SnapToGrid();
        DetectInteractable();
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void UpdateVisibility()
    {
        _spriteRenderer.enabled = player.EquippedToolType != ToolType.None || showOnNoTool;
    }

    private void SnapToGrid()
    {
        // Bỏ qua nếu chuột ngoài màn hình
        if (Input.mousePosition.x < 0 || Input.mousePosition.x > Screen.width ||
            Input.mousePosition.y < 0 || Input.mousePosition.y > Screen.height) return;

        Vector3 mouseWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        Vector2Int mouseCell = new Vector2Int(
            Mathf.FloorToInt(mouseWorld.x),
            Mathf.FloorToInt(mouseWorld.y)
        );

        Vector2Int playerCell = new Vector2Int(
            Mathf.FloorToInt(player.transform.position.x + playerGridOffset),
            Mathf.FloorToInt(player.transform.position.y + playerGridOffset)
        );

        int range = player.EquippedToolRange;
        int targetX = Mathf.Clamp(mouseCell.x, playerCell.x - range, playerCell.x + range);
        int targetY = Mathf.Clamp(mouseCell.y, playerCell.y - range, playerCell.y + range);

        transform.position = new Vector3(
            targetX + indicatorGridOffset,
            targetY + indicatorGridOffset,
            0f
        );
    }

    private void DetectInteractable()
    {
        Collider2D hit = Physics2D.OverlapPoint(transform.position, interactableLayerMask);

        IInteractable newInteractable = hit != null ? hit.GetComponent<IInteractable>() : null;

        if (newInteractable == _currentInteractable) return;

        _currentInteractable?.OnIndicatorExit();
        newInteractable?.OnIndicatorEnter();
        _currentInteractable = newInteractable;

        OnSelectedResourceNodeChanged?.Invoke(this, new SelectedResourceChangedEventArgs
        {
            SelectedResource = _currentInteractable
        });
    }
}
