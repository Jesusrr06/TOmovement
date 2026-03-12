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
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Debug.Log("PlayerControls started, Rigidbody assigned: " + (rb != null));
    }


    void OnMove(InputValue movementValue)
    {
        movementInput = movementValue.Get<Vector2>();
        Debug.Log("OnMove input: " + movementInput);
    }

    void OnJump(InputValue value)
    {
        Debug.Log("OnJump called, isPressed: " + value.isPressed);
        if (value.isPressed && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            Debug.Log("Jump force applied");
        }
    }

    private void Update()
    {
        if (rb.linearVelocity.y < 0.1f)  // Check only when falling downward
        {
            isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
        }
        else
        {
            isGrounded = false;  // Not checking, so assume not grounded
        }
        // Ground check
        Debug.Log("Is grounded: " + isGrounded);
    }

    private void FixedUpdate()
    {
        // Calculate movement direction relative to player orientation
        Vector3 forward = transform.forward * movementInput.y;
        Vector3 right = transform.right * movementInput.x;
        Vector3 movement = (forward + right).normalized * speed;

        // Set velocity directly for precise control
        rb.linearVelocity = new Vector3(movement.x, rb.linearVelocity.y, movement.z);
    }
}