using System;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // ----------------------------------------------------------
    // Singleton
    // ----------------------------------------------------------
    public static Player Instance { get; private set; }

    public SeedSO EquippedSeed { get; private set; }

    // ----------------------------------------------------------
    // Events
    // ----------------------------------------------------------
    public event EventHandler<ToolUsedEventArgs> OnToolUsed;

    public class ToolUsedEventArgs : EventArgs
    {
        public ToolType ToolType;
    }

    // ----------------------------------------------------------
    // Inspector fields
    // ----------------------------------------------------------
    [Header("References")]
    [SerializeField] private GameInput gameInput;
    [SerializeField] private PlayerIndicator playerIndicator;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private ToolSO equippedTool;


    // ----------------------------------------------------------
    // Private state
    // ----------------------------------------------------------
    private Rigidbody2D _rb;
    private Vector2 _inputVector;
    private ResourceNode _targetResource;
    private bool _isPointerOverUI;

    // ----------------------------------------------------------
    // Read-only properties
    // ----------------------------------------------------------
    public bool IsMoving => _inputVector != Vector2.zero;
    public Vector2 InputVector => _inputVector;

    public ToolType EquippedToolType => equippedTool.toolType;

    public ToolSO EquippedToolSO => equippedTool;

    public int EquippedToolRange => equippedTool.interactRange;

    public void ChangeEquippedTool(ToolSO newTool)
    {
        equippedTool = newTool;
    }

    public void EquipSeed(SeedSO seed)
    {
        EquippedSeed = seed;
    }

    // ----------------------------------------------------------
    // Unity lifecycle
    // ----------------------------------------------------------
    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        gameInput.OnUseToolAction += OnUseToolInputReceived;
        playerIndicator.OnSelectedResourceNodeChanged += OnSelectedResourceChanged;
    }

    private void Update()
    {
        _isPointerOverUI = EventSystem.current.IsPointerOverGameObject();
        _inputVector = gameInput.GetMovementVectorNormalized();
        
    }

    private void FixedUpdate()
    {
        _rb.MovePosition(_rb.position + _inputVector * moveSpeed * Time.fixedDeltaTime);
    }

    // ----------------------------------------------------------
    // Public API
    // ----------------------------------------------------------

    /// <summary>Called by PlayerAnimator at the animation's
    /// action frame to apply the tool effect.</summary>
    public void PerformToolAction()
    {
        if (_isPointerOverUI) return;
        _targetResource?.TakeDamage(equippedTool);
    }

    /// <summary>Clears the target when the resource is destroyed.</summary>
    public void ClearTargetResource()
    {
        _targetResource = null;
    }

    

    // ----------------------------------------------------------
    // Private event handlers
    // ----------------------------------------------------------
    private void OnUseToolInputReceived(object sender, EventArgs e)
    {
        if (_isPointerOverUI) return;

        if (IsMoving) return;

        OnToolUsed?.Invoke(this, new ToolUsedEventArgs { ToolType = EquippedToolType });

        // Nếu tay không (None) thì không có animation event
        // → gọi PerformToolAction trực tiếp luôn
        if (EquippedToolType == ToolType.None)
        {
            PerformToolAction();
        }
    }

    private void OnSelectedResourceChanged(object sender, PlayerIndicator.SelectedResourceChangedEventArgs e)
    {
        _targetResource = e.SelectedResource;
    }
}
