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
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float gravity = 9.81f;

    private Vector2 moveInput;
    private float verticalVelocity;

    void Start()
    { 
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Debug.Log(moveInput);
        HandleMovement();
        HandleJump();
        ApplyMovement();
        HandleRotation();
        HandleAnimations();
    }

    // ---------------- MOVEMENT ----------------
    void HandleMovement()
    {
        // Solo input horizontal
    }

    void ApplyMovement()
    {
        Vector3 input = new Vector3(moveInput.x, 0, moveInput.y);
        Vector3 move = Camera.main.transform.TransformDirection(input);

        move *= movementSpeed;

        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);
    }

    // ---------------- ROTATION ----------------
    void HandleRotation()
    {
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    // ---------------- JUMP ----------------
    void HandleJump()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame && characterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    // ---------------- ANIMATIONS ----------------
    void HandleAnimations()
    {
        bool grounded = characterController.isGrounded;
        Vector3 direction = new Vector3(moveInput.x, 0, moveInput.y);

        animator.SetBool("IsRunning", direction.sqrMagnitude > 0.01f);
        animator.SetBool("IsJumping", !grounded && verticalVelocity > 0);
        animator.SetBool("IsFalling", !grounded && verticalVelocity < 0);
    }

    // ---------------- INPUT SYSTEM ----------------
    public void OnMove(InputValue value)
    {
        Debug.Log("FUNCIONA INPUT");
        moveInput = value.Get<Vector2>();
    }
}