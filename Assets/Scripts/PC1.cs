using UnityEngine;
using UnityEngine.InputSystem;

public class PC1 : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpSpeed;
    [SerializeField] private float gravity;

    private Vector2 moveInput;
    private float verticalVelocity;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * movementSpeed;
        move.y = verticalVelocity;

        characterController.Move(move * Time.deltaTime);

        // ROTATION
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

        // ANIMATIONS
        bool grounded = characterController.isGrounded;

        animator.SetBool("IsJumping", !grounded && verticalVelocity > 0);
        animator.SetBool("IsFalling", !grounded && verticalVelocity < 0);
        animator.SetBool("IsRunning", direction.sqrMagnitude > 0.01f);
    }

    void Move()
    {
        // vacío o puedes eliminarlo
    }

    void Jump()
    {
        if (Keyboard.current.digit0Key.wasPressedThisFrame && characterController.isGrounded)
        {
            verticalVelocity = jumpSpeed;
        }

        verticalVelocity -= gravity * Time.deltaTime;

        if (characterController.isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -2f;
        }
    }

    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        characterController.Move(moveInput * Time.deltaTime*movementSpeed);
    }
}