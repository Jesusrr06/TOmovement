using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PLmovement : MonoBehaviour
{
    public float walkSpeed = 4f;
    public float runMultiplier = 1.8f;
    public float jumpHeight = 1.2f;
    public float gravity = -9.81f;
    public float turnSmoothTime = 0.08f;
    public Transform cameraTransform; // optional: set to main camera for camera-relative movement

    CharacterController cc;
    Vector3 velocity;
    float turnSmoothVelocity;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        if (cameraTransform == null && Camera.main != null) cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Read movement input (keyboard WASD or left stick)
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 input = new Vector3(h, 0f, v);

        float inputMag = Mathf.Clamp01(input.magnitude);

        if (inputMag > 0.01f)
        {
            Vector3 moveDir;
            if (cameraTransform != null)
            {
                // Move relative to camera forward/right
                Vector3 forward = cameraTransform.forward;
                Vector3 right = cameraTransform.right;
                forward.y = 0f;
                right.y = 0f;
                forward.Normalize();
                right.Normalize();
                moveDir = forward * v + right * h;
            }
            else
            {
                // Move relative to player orientation
                moveDir = transform.forward * v + transform.right * h;
            }

            moveDir.Normalize();

            // Smoothly rotate player toward movement direction
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float speed = walkSpeed * (Input.GetButton("Sprint") ? runMultiplier : 1f);
            cc.Move(moveDir * speed * inputMag * Time.deltaTime);
        }

        // Ground and jump handling
        if (cc.isGrounded && velocity.y < 0f)
        {
            velocity.y = -2f; // small downward force to keep grounded
        }

        if (Input.GetButtonDown("Jump") && cc.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
