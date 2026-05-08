using UnityEngine;
using UnityEngine.InputSystem;

public class PC : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Combat")]
    [SerializeField] private bool canMoveWhilePunching = false;

    private Vector2 moveInput;
    private Vector3 moveDirection;
   
    [Header("Player Info")]
    public int playerId;
    
    
    private float verticalVelocity;

    private bool jumpPressed;
    private bool isPunching;
    private Camera camera1;

    void Start()
    {
        camera1 = Camera.main;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        HandleGravity();
        HandleMovement();
        HandleRotation();
        HandleAnimations();
    }

    // =====================================
    // MOVEMENT
    // =====================================
    void HandleMovement()
    {
        // Disable movement while punching
        if (isPunching && !canMoveWhilePunching)
        {
            moveDirection = Vector3.zero;
        }
        else
        {
            // Camera-relative movement
            Vector3 camForward = camera1.transform.forward;
            Vector3 camRight = camera1.transform.right;

            camForward.y = 0;
            camRight.y = 0;

            camForward.Normalize();
            camRight.Normalize();

            moveDirection =
                camForward * moveInput.y +
                camRight * moveInput.x;

            moveDirection *= movementSpeed;
        }

        // Jump
        if (jumpPressed && characterController.isGrounded && !isPunching)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

            // consume jump input
            jumpPressed = false;
        }

        // Apply gravity
        moveDirection.y = verticalVelocity;

        // Move player
        characterController.Move(moveDirection * Time.deltaTime);
    }

    // =====================================
    // ROTATION
    // =====================================
    void HandleRotation()
    {
        Vector3 horizontalDirection = moveDirection;
        horizontalDirection.y = 0;

        if (horizontalDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(horizontalDirection);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // =====================================
    // GRAVITY
    // =====================================
    void HandleGravity()
    {
        // Small downward force while grounded
        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
    }

    // =====================================
    // ANIMATIONS
    // =====================================
    void HandleAnimations()
    {
        Vector3 horizontalMove = moveDirection;
        horizontalMove.y = 0;

        bool isRunning = horizontalMove.sqrMagnitude > 0.1f;

        animator.SetBool("IsRunning", isRunning);

        animator.SetBool(
            "IsJumping",
            !characterController.isGrounded && verticalVelocity > 0
        );

        animator.SetBool(
            "IsFalling",
            !characterController.isGrounded && verticalVelocity < 0
        );

        animator.SetBool("IsPunching", isPunching);
    }

    // =====================================
    // INPUT SYSTEM
    // =====================================

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    public void OnJump(InputValue value)
    {
        if (value.isPressed)
        {
            jumpPressed = true;
        }
    }

    public void OnPunch(InputValue value)
    {
        if (value.isPressed && !isPunching)
        {
            isPunching = true;
            animator.SetTrigger("Punch");
        }
    }

    // =====================================
    // ANIMATION EVENT
    // Add this at the END of the punch animation
    // =====================================

    public void EndPunch()
    {
        isPunching = false;
    }
}