using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float dashSpeed = 12f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 lastMoveInput;
    private Vector2 dashDirection;
    private Animator animator;
    private bool isDashing;
    private float dashTimeRemaining;
    private float dashCooldownRemaining;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (dashCooldownRemaining > 0f)
        {
            dashCooldownRemaining = Mathf.Max(0f, dashCooldownRemaining - Time.fixedDeltaTime);
        }

        if (isDashing)
        {
            rb.linearVelocity = dashDirection * dashSpeed;
            dashTimeRemaining -= Time.fixedDeltaTime;
            if (dashTimeRemaining <= 0f)
            {
                isDashing = false;
            }
            return;
        }

        rb.linearVelocity = moveInput * moveSpeed;
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        bool isWalking = input.sqrMagnitude > 0.0001f;

        if (isWalking)
        {
            lastMoveInput = input;
        }

        moveInput = input;
        animator.SetBool("isWalking", isWalking);
        animator.SetFloat("InputX", moveInput.x);
        animator.SetFloat("InputY", moveInput.y);

        if (context.canceled)
        {
            animator.SetBool("isWalking", false);
            animator.SetFloat("LastInputX", lastMoveInput.x);
            animator.SetFloat("LastInputY", lastMoveInput.y);
        }
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (!context.performed || isDashing || dashCooldownRemaining > 0f)
        {
            return;
        }

        Vector2 dashInput = moveInput.sqrMagnitude > 0.0001f ? moveInput.normalized : lastMoveInput.normalized;
        if (dashInput.sqrMagnitude <= 0.0001f)
        {
            return;
        }

        isDashing = true;
        dashDirection = dashInput;
        dashTimeRemaining = dashDuration;
        dashCooldownRemaining = dashCooldown;
    }
}
