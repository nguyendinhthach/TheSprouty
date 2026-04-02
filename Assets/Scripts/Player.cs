using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;
    [SerializeField] private float moveSpeed = 5.0f;

    private Rigidbody2D rb;

    private Vector2 inputVector;
    private bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        inputVector = gameInput.GetMovementVectorNormalized();
        isMoving = inputVector != Vector2.zero;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * moveSpeed * Time.fixedDeltaTime);
    }

    public Vector2 GetInputVector()
    {
        return inputVector;
    }

    public bool IsMoving()
    {
        return isMoving;
    }
}
