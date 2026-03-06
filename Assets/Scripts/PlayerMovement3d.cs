using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement3d : MonoBehaviour
{
    public float speed = 5f;
    public float acceleration = 20f;
    public float deceleration = 40f;
    public float sprintMultiplier = 1.8f;
    public Transform respawnPoint;
    public float fallThreshold = -10f;
    public float jumpForce = 7f;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask = ~0;
    public float coyoteTime = 0.15f;
    public int extraJumps = 1;
    
    public InputActionAsset inputActions; // Drag your InputSystem_Actions here
    public PlayerInput playerInput; // Reference to PlayerInput component

    public Rigidbody rb;
    private float movementX;
    private float movementY;
    private bool isSprinting;
    private bool jumpPressed;
    private bool isGrounded;
    private float coyoteTimer;
    private int jumpsLeft;
    private Vector3 startPosition;
    
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPosition = transform.position;

        // Get PlayerInput component
        if (playerInput == null)
        {
            playerInput = GetComponent<PlayerInput>();
        }

        // Use PlayerInput's actions if available, otherwise use inputActions asset
        InputActionAsset actionsToUse = null;
        
        if (playerInput != null && playerInput.actions != null)
        {
            actionsToUse = playerInput.actions;
            Debug.Log("Using PlayerInput component's actions");
        }
        else if (inputActions != null)
        {
            actionsToUse = inputActions;
            Debug.Log("Using InputActionAsset directly");
        }
        else
        {
            Debug.LogError("✗ No input actions found! Assign InputActionAsset or add PlayerInput component.");
            return;
        }

        // Get actions from asset
        try
        {
            moveAction = actionsToUse.FindAction("Move");
            sprintAction = actionsToUse.FindAction("Sprint");
            jumpAction = actionsToUse.FindAction("Jump");

            if (moveAction != null && sprintAction != null && jumpAction != null)
            {
                Debug.Log("✓ All input actions found!");
            }
            else
            {
                Debug.LogError("✗ One or more actions not found. Check your InputSystem_Actions file.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading input actions: " + e.Message);
        }
    }

    void OnEnable()
    {
        if (moveAction != null && sprintAction != null && jumpAction != null)
        {
            // Enable the actions
            moveAction.Enable();
            sprintAction.Enable();
            jumpAction.Enable();

            // Subscribe to the input callbacks
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
            sprintAction.performed += OnSprint;
            sprintAction.canceled += OnSprint;
            jumpAction.performed += OnJump;
            jumpAction.canceled += OnJump;
        }
    }

    void OnDisable()
    {
        if (moveAction != null && sprintAction != null && jumpAction != null)
        {
            // Disable the actions
            moveAction.Disable();
            sprintAction.Disable();
            jumpAction.Disable();

            // Unsubscribe from input callbacks
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            sprintAction.performed -= OnSprint;
            sprintAction.canceled -= OnSprint;
            jumpAction.performed -= OnJump;
            jumpAction.canceled -= OnJump;
        }
    }


    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        Debug.Log($"Move Input: X={movementX}, Y={movementY}");
    }

    void OnSprint(InputAction.CallbackContext context)
    {
        isSprinting = context.ReadValueAsButton();
        Debug.Log($"Sprint: {isSprinting}");
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jumpPressed = true;
            Debug.Log("Jump Pressed!");
        }
    }

    private void FixedUpdate()
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody is NULL!");
            return;
        }

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
            
            // Debug
            Debug.Log($"Input: {input}, Speed: {targetSpeed}, isSprinting: {isSprinting}");
        }
        else
        {
            desiredVelocity = Vector3.zero;
        }

        Vector3 velocityDiff = desiredVelocity - currentVelocityXZ;

        float maxChange = (input.sqrMagnitude > 0.0001f ? acceleration : deceleration) * Time.fixedDeltaTime;
        if (velocityDiff.magnitude > maxChange)
            velocityDiff = velocityDiff.normalized * maxChange;

        rb.AddForce(velocityDiff, ForceMode.VelocityChange);

        // Handle jump: allow jump while grounded (or within coyote time), or use extra jumps in air
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

        // Respawn if player falls below threshold
        if (transform.position.y < fallThreshold)
            Respawn();
    }

    void Update()
    {
        // Input is processed in FixedUpdate for consistency with physics
    }

    private void Respawn()
    {
        Vector3 spawnPos = respawnPoint != null ? respawnPoint.position : startPosition;
        transform.position = spawnPos;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

}
