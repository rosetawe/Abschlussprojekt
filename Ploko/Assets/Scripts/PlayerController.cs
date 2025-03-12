using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float runSpeed = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool isRunning;
    private bool isGrounded;

    private void Awake()
    {
        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        playerControls.Enable();
        playerControls.Movement.Jump.performed += _ => Jump();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }

    private void Update()
    {
        PlayerInput();
        CheckGrounded();
        Debug.Log("Is Grounded: " + isGrounded);
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
        Jump();
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();
        isRunning = playerControls.Movement.Run.ReadValue<float>() > 0;

        animator.SetFloat("moveX", movement.x);
        animator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        float speed = isRunning ? moveSpeed * runSpeed : moveSpeed;
        rb.MovePosition(rb.position + movement * (speed * Time.fixedDeltaTime));
    }

    private void Jump()
    {
        if (isGrounded)
        {
            Debug.Log("Jump triggered!");
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        else
        {
            Debug.Log("Jump ignored, not grounded.");
        }
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        if (collider != null)
        {
            Debug.Log("Ground detected: " + collider.gameObject.name);
        }
    }
}