using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement3d : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 20f; // units per second^2 when input is present
    public float deceleration = 40f; // units per second^2 when no input (stopping)
    public float sprintMultiplier = 1.8f; // multiply speed while sprinting
    public Transform respawnPoint; // optional: set a Transform in the scene to respawn at
    public float fallThreshold = -10f; // Y position below which the player respawns
    public float jumpForce = 7f; // initial upward velocity applied when jumping
    public float groundCheckDistance = 0.2f; // distance for ground raycast
    public LayerMask groundMask = ~0; // layers considered ground (default: everything)
    public float coyoteTime = 0.15f; // allow jump shortly after leaving ground
    public int extraJumps = 1; // number of extra jumps in air (1 = double jump)

    private Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isSprinting;
    private bool jumpPressed;
    private bool isGrounded;
    private float coyoteTimer;
    private int jumpsLeft;
    private Vector3 startPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    // Input System: add a "Sprint" action and bind it to this callback (hold-to-sprint)
    void OnSprint(InputValue value)
    {
        // For button actions, Get<float>() returns 1 when pressed
        isSprinting = value.Get<float>() > 0.5f;
    }

    // Input System: jump action (press to jump). Bind your Jump action to this.
    void OnJump(InputValue value)
    {
        // Button-style action: 1 when pressed.
        jumpPressed = value.Get<float>() > 0.5f;
    }

    private void FixedUpdate()
    {
        Vector3 input = new Vector3(movementX, 0f, movementY);
        Vector3 currentVelocity = rb.linearVelocity;
        Vector3 currentVelocityXZ = new Vector3(currentVelocity.x, 0f, currentVelocity.z);

        // Ground check (simple raycast down from the transform position)
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        // Update coyote timer and reset available jumps when grounded
        if (isGrounded)
        {
            coyoteTimer = coyoteTime;
            jumpsLeft = extraJumps;
        }
        else
        {
            coyoteTimer -= Time.fixedDeltaTime;
        }

        Vector3 desiredVelocity;
        if (input.sqrMagnitude > 0.0001f)
        {
            // Preserve existing speed (momentum) when turning: don't force a lower target speed
            float currentSpeed = currentVelocityXZ.magnitude;
            float speedMultiplier = isSprinting ? sprintMultiplier : 1f;
            float targetSpeed = Mathf.Max(currentSpeed, speed * speedMultiplier * input.magnitude);
            desiredVelocity = input.normalized * targetSpeed;
        }
        else
        {
            desiredVelocity = Vector3.zero;
        }

        Vector3 velocityDiff = desiredVelocity - currentVelocityXZ;

        float maxChange = (input.sqrMagnitude > 0.0001f ? acceleration : deceleration) * Time.fixedDeltaTime;
        if (velocityDiff.magnitude > maxChange)
            velocityDiff = velocityDiff.normalized * maxChange;
if (jumpPressed)
        {
            bool didJump = false;

            if (isGrounded || coyoteTimer > 0f)
            {
                Vector3 lv = rb.linearVelocity;
                lv.y = jumpForce;
                rb.linearVelocity = lv;
                didJump = true;
                // consume coyote so you can't double-dip
                coyoteTimer = 0f;
            }
            else if (jumpsLeft > 0)
            {
                Vector3 lv = rb.linearVelocity;
                lv.y = jumpForce;
                rb.linearVelocity = lv;
                jumpsLeft--;
                didJump = true;
            }

            if (didJump)
                jumpPressed = false;
        }
        rb.AddForce(velocityDiff, ForceMode.VelocityChange);

        // Handle jump: allow jump while grounded (or within coyote time), or use extra jumps in air
        

        // Respawn if player falls below threshold
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    private void Respawn()
    {
        Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : startPosition;
        transform.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
