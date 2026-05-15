using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;

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
    private Vector2 _moveInput;
    private float _verticalVelocity;
    private Animator _animator;
    public  int playerId;
    public bool IsGrounded => _characterController.isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cam = Camera.main;
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {      
        HandleGravity(); 
        HandlePlayerInput();
        HandleMovement();
        HandleRotation();
        HandleAnimations();
    }

    // ================= INPUT FROM CONTROLLER =================
    private void HandlePlayerInput()
    {            Vector2 input = Vector2.zero;

     
 
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
        
        
   
        SetInput(input.normalized);
    }

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

        _characterController.Move(_moveDirection * Time.deltaTime);    }

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

    private void HandleGravity()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        _verticalVelocity += gravity * Time.deltaTime;
    }

    // ================= ACTIONS =================
    private void Jump()
    {
        if (isStunned || !_characterController.isGrounded) return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // ================= EXTERNAL EFFECTS =================
   
    public void ApplyStun(float duration, StunType type)
    {
        StopCoroutine(nameof(StunRoutine));
        StartCoroutine(StunRoutine(duration, type));
    }

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
    private void HandleAnimations()
    {
        Vector3 horizontal = _moveDirection;
        horizontal.y = 0;
        float speed = horizontal.magnitude;


        bool isRunning = speed > 0.1f;
        bool isJumping = !_characterController.isGrounded && _verticalVelocity > 0.1f;
        bool isFalling = !_characterController.isGrounded && _verticalVelocity < -0.1f;

        _animator.SetBool("IsRunning", isRunning && !isStunned);
        _animator.SetBool("IsJumping", isJumping);
        _animator.SetBool("IsFalling", isFalling);
        _animator.SetBool("IsStunned", isStunned);

    }
    }
