using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------
    public event EventHandler OnUseToolAction;

    // ----------------------------------------------------------
    // Private state  (ENCAPSULATION)
    // ----------------------------------------------------------
    private InputSystem_Actions _inputActions;

    private void Awake()
    {
        _inputActions = new InputSystem_Actions();
        _inputActions.Player.Enable();
        _inputActions.Player.UseTool.performed += OnUseToolPerformed;
    }

    private void OnDestroy()
    {
        // Always unsubscribe to avoid memory leaks
        _inputActions.Player.UseTool.performed -= OnUseToolPerformed;
        _inputActions.Dispose();
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------
    public Vector2 GetMovementVectorNormalized()
    {
        return _inputActions.Player.Move.ReadValue<Vector2>().normalized;
    }

    // ----------------------------------------------------------
    // Private handlers
    // ----------------------------------------------------------
    private void OnUseToolPerformed(InputAction.CallbackContext ctx)
    {
        OnUseToolAction?.Invoke(this, EventArgs.Empty);
    }
}
