using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControls : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody rb;
    private Vector2 movementInput;
    private bool isGrounded;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    private DefaultInputActions.PlayerActions actions;
    public float turnSpeed = 5f;
    private Quaternion targetRotation;
    private bool isTurning;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetRotation = transform.rotation;

        Debug.Log("PlayerControls started, Rigidbody assigned: " + (rb != null));
    }


    private void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
        Debug.Log("OnMove input: " + movementInput);
    }

    private void OnJump(InputValue value)
    {
        Debug.Log("OnJump called, isPressed: " + value.isPressed);
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        if (value.isPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jump force applied");
        }
    }




    private void FixedUpdate()
    {
        // Calculate movement direction relative to player orientation
        Vector3 forward = transform.forward * movementInput.y;
        Vector3 right = transform.right * movementInput.x;
        Vector3 movement = (forward + right).normalized * speed;
        bool jumpPressed = Keyboard.current.spaceKey.isPressed; // Check jump input directly in FixedUpdate
        if (jumpPressed && isGrounded) // Check jump input in FixedUpdate for better timing
        {
            movement = transform.up * jumpForce; // Simulate jump input

        }

        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }


    private void Update()
    {
        // Handle player rotation based on movement input
        if (movementInput != Vector2.zero)
        {
            Vector3 direction = new Vector3(movementInput.x, 0, movementInput.y);
            targetRotation = Quaternion.LookRotation(direction);
            isTurning = true;
        }

        if (isTurning)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turnSpeed * Time.deltaTime);
            if (Quaternion.Angle(transform.rotation, targetRotation) < 1f)
            {
                isTurning = false; // Stop turning when close enough to target rotation
            }
        }
    }
}