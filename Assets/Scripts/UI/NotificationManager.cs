// ──────────────────────────────────────────────
// TheSprouty | UI/NotificationManager.cs
// Singleton. Manages notification popups with stacking logic.
// Same item within mergeWindow → update quantity instead of new popup.
// ──────────────────────────────────────────────
using System.Collections;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static NotificationManager Instance { get; private set; }

    // ----------------------------------------------------------
    // Serialized fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private NotificationUI notificationUI;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 3f;
    [SerializeField] private float mergeWindow = 2f;
    [SerializeField] private float hideBuffer = 1f;

    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private ItemSO _currentItem;
    private int _currentQuantity;
    private float _mergeTimer;
    private float _displayTimer;
    private bool _isShowing;
    private string _currentMessage;
    private Coroutine _timerRoutine;

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Show item pickup notification. Stacks if same item within merge window.</summary>
    public void ShowItemNotification(ItemSO item, int quantity)
    {
        // Same item within merge window → stack
        if (_isShowing && _currentItem == item && _mergeTimer > 0f)
        {
            _currentQuantity += quantity;
            _displayTimer = displayDuration;
            _mergeTimer = mergeWindow; // reset merge window khi cùng item
            notificationUI.UpdateText(FormatItemText(_currentItem, _currentQuantity));
            return;
        }

        // New notification
        ShowNew(item, quantity);
    }

    /// <summary>Show a plain text notification (e.g. "Inventory Full!").</summary>
    public void ShowMessage(string text)
    {
        // Đang hiện cùng message → bỏ qua, không reset timer
        if (_isShowing && _currentMessage == text) return;

        _currentItem = null;
        _currentQuantity = 0;
        _currentMessage = text;
        _displayTimer = displayDuration;
        _isShowing = true;

        notificationUI.Show(text);

        if (_timerRoutine != null) StopCoroutine(_timerRoutine);
        _timerRoutine = StartCoroutine(DisplayTimerRoutine());
    }

    // ----------------------------------------------------------
    // Private methods
    // ----------------------------------------------------------
    private void ShowNew(ItemSO item, int quantity)
    {
        _currentItem = item;
        _currentQuantity = quantity;
        _currentMessage = null; // clear message khi show item notification
        _mergeTimer = mergeWindow;
        _displayTimer = displayDuration;
        _isShowing = true;

        notificationUI.Show(FormatItemText(item, quantity));

        if (_timerRoutine != null) StopCoroutine(_timerRoutine);
        _timerRoutine = StartCoroutine(DisplayTimerRoutine());
    }

    private IEnumerator DisplayTimerRoutine()
    {
        while (_displayTimer > 0f)
        {
            _displayTimer -= Time.deltaTime;
            if (_mergeTimer > 0f) _mergeTimer -= Time.deltaTime;
            yield return null;
        }

        notificationUI.Hide();
        yield return new WaitForSeconds(hideBuffer); // chờ fade-out xong

        _isShowing = false;
        _currentItem = null;
        _currentMessage = null;
    }

    private string FormatItemText(ItemSO item, int quantity)
    {
        return quantity > 1 ? $"{item.itemName} x{quantity}" : item.itemName;
    }
}
