using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

/// <summary>
/// Handles player movement: walking, rotation, jumping, gravity and stun state.
/// Also exposes grounded checks and animation state updates.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    private CharacterController _characterController;
    private Camera _cam;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump / Gravity")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [FormerlySerializedAs("_isStunned")] [Header("Stun")]
    public bool isStunned=false;
    public enum StunType
    {
        Hitstun,
        Blockstun
    }

    private Vector3 _moveDirection;
    private Vector3 _externalVelocity;
    private Coroutine _knockbackCoroutine;    private Vector2 _moveInput;
    private float _verticalVelocity;
    private Animator _animator;
    public  int playerId;
    public bool IsGrounded => _characterController.isGrounded;

    /// <summary>
    /// Initialize component references: CharacterController, Camera and Animator.
    /// Also enables the CharacterController if found so movement works on spawn.
    /// </summary>
    private void Awake()
    {
        Debug.Log($"{gameObject.name} ACTIVE: {gameObject.activeInHierarchy}");
        _characterController = GetComponentInChildren<CharacterController>();

        if (_characterController == null)
        {
            Debug.LogError("No CharacterController found on " + gameObject.name);
        }
        else
        {
            Debug.Log($"CC enabled: {_characterController.enabled}");
            _characterController.enabled = true;
        }
        _cam = Camera.main;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Per-frame update: gravity, input, movement, rotation and animation handling.
    /// </summary>
    private void Update()
    {      
     
        HandleGravity(); 
        HandlePlayerInput();
        HandleMovement();
        HandleRotation();
        HandleAnimations();
    }

    // ================= INPUT FROM CONTROLLER =================
    /// <summary>
    /// Reads keyboard input for the current playerId and converts it into a normalized move vector.
    /// Ignores input while stunned or if the CharacterController is missing/disabled.
    /// </summary>
    private void HandlePlayerInput()
    {
        if (_characterController is null || !_characterController.enabled)
            return;
        Vector2 input = Vector2.zero;


        if (!Input.GetKey(KeyCode.E))
        {
        

            // PLAYER 1
            if (playerId == 1)
            {
                if (Input.GetKey(KeyCode.W)) input.y += 1;
                if (Input.GetKey(KeyCode.S)) input.y -= 1;
                if (Input.GetKey(KeyCode.A)) input.x -= 1;
                if (Input.GetKey(KeyCode.D)) input.x += 1;

                if (Input.GetKeyDown(KeyCode.Space))
                    Jump();
            }
        }

        if (!Input.GetKey(KeyCode.Keypad4))
        {
            // PLAYER 2
            if (playerId == 2)
            {
                if (Input.GetKey(KeyCode.UpArrow)) input.y += 1;
                if (Input.GetKey(KeyCode.DownArrow)) input.y -= 1;
                if (Input.GetKey(KeyCode.LeftArrow)) input.x -= 1;
                if (Input.GetKey(KeyCode.RightArrow)) input.x += 1;

                if (Input.GetKeyDown(KeyCode.Keypad0))
                    Jump();
            }
        }



        SetInput(input.normalized);
    }

    /// <summary>
    /// Applies processed input to the movement state. Clears input while stunned.
    /// </summary>
    /// <param name="input">Normalized 2D input vector (x:right, y:forward)</param>
    private void SetInput(Vector2 input)
    {
        if (isStunned)
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = input;
    }
    

    // ================= MOVEMENT =================
    /// <summary>
    /// Converts the current 2D input into a 3D movement vector relative to the camera,
    /// applies gravity/vertical velocity and moves the CharacterController.
    /// </summary>
    private void HandleMovement()
    {
        if (isStunned)
        {
            _moveDirection = Vector3.zero;
        }
        else
        {
            Vector3 forward = _cam.transform.forward;
            Vector3 right = _cam.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            _moveDirection =
                forward * _moveInput.y +
                right * _moveInput.x;

            _moveDirection *= movementSpeed;
        }

        _moveDirection.y = _verticalVelocity;

        // Apply any external velocities such as knockback on top of player-controlled movement
        Vector3 finalMove = _moveDirection + _externalVelocity;
        _characterController.Move(finalMove * Time.deltaTime);
    }

    /// <summary>
    /// Smoothly rotates the player towards movement direction when moving.
    /// </summary>
    private void HandleRotation()
    {
        Vector3 horizontal = _moveDirection;
        horizontal.y = 0;

        if (horizontal.sqrMagnitude > 0.01f)
        {
            Quaternion target = Quaternion.LookRotation(horizontal);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                target,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// Applies simple gravity integration and keeps the vertical velocity clamped while grounded.
    /// </summary>
    private void HandleGravity()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        _verticalVelocity += gravity * Time.deltaTime;
    }

    // ================= ACTIONS =================
    /// <summary>
    /// Initiates a jump by setting the vertical velocity using the configured jump height.
    /// Will not jump while stunned or when not grounded.
    /// </summary>
    private void Jump()
    {
        if (isStunned || !_characterController.isGrounded) return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // ================= EXTERNAL EFFECTS =================
   
    /// <summary>
    /// Public API to apply a stun effect to this player. Stops movement for the duration.
    /// </summary>
    /// <param name="duration">Duration of the stun in seconds.</param>
    /// <param name="type">Type of stun (Hitstun or Blockstun) — can be used to vary behavior.</param>
    public void ApplyStun(float duration, StunType type)
    {
        StopCoroutine(nameof(StunRoutine));
        StartCoroutine(StunRoutine(duration, type));
    }

    /// <summary>
    /// Coroutine that toggles the isStunned flag for the given duration.
    /// Different stun types may be applied to change timing or allowed actions while stunned.
    /// </summary>
    /// <returns>IEnumerator used by StartCoroutine.</returns>
    private IEnumerator StunRoutine(float duration, StunType type)
    {
        if (type == StunType.Hitstun)
        {
            isStunned = true;

            yield return new WaitForSeconds(duration);

            isStunned = false;
        }
        else if (type == StunType.Blockstun)
        {
            // blockstun podría ser más corto o permitir block
            isStunned = true;

            yield return new WaitForSeconds(duration);

            isStunned = false;
        }
    }

    /// <summary>
    /// Apply an external knockback velocity for a short duration. External velocity is added on top of regular movement.
    /// </summary>
    public void ApplyKnockback(Vector3 velocity, float duration)
    {
        Debug.Log($"ApplyKnockback called on {gameObject.name}: velocity={velocity}, duration={duration}");
        if (_knockbackCoroutine != null)
            StopCoroutine(_knockbackCoroutine);
        _knockbackCoroutine = StartCoroutine(KnockbackRoutine(velocity, duration));
    }

    private IEnumerator KnockbackRoutine(Vector3 velocity, float duration)
    {
        Debug.Log($"KnockbackRoutine start on {gameObject.name}: {velocity} for {duration}");
        _externalVelocity = velocity;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        _externalVelocity = Vector3.zero;
        _knockbackCoroutine = null;
        Debug.Log($"KnockbackRoutine end on {gameObject.name}");
    }

    /// <summary>
    /// Updates animator parameters based on movement state (running, jumping, falling, stunned).
    /// </summary>
    private void HandleAnimations()
    {
        Vector3 horizontal = _moveDirection;
        horizontal.y = 0;
        float speed = horizontal.magnitude;


        bool isRunning = speed > 0.1f && _characterController.isGrounded;
        bool isJumping = !_characterController.isGrounded && _verticalVelocity > 0.1f;
        bool isFalling = !_characterController.isGrounded && _verticalVelocity < -0.1f;

        _animator.SetBool("IsRunning", isRunning && !isStunned );
        _animator.SetBool("IsJumping", isJumping);
        _animator.SetBool("IsFalling", isFalling);
        _animator.SetBool("IsStunned", isStunned);

    }
    }
