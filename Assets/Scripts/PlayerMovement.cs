using UnityEngine;
using System.Collections;

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

    [Header("Stun")]
    private bool _isStunned=false;

    [Header("Knockback")]
    [SerializeField] private float knockbackDamping = 5f;
    private Vector3 _externalForce;

    private Vector3 _moveDirection;
    private Vector2 _moveInput;
    private float _verticalVelocity;
public  int playerId;
    public bool IsGrounded => _characterController.isGrounded;

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _cam = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleGravity();
    }

    // ================= INPUT FROM CONTROLLER =================
    public void SetInput(Vector2 input)
    {
        if (_isStunned)
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = input;
    }

    // ================= MOVEMENT =================
    private void HandleMovement()
    {
        if (_isStunned)
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

        Vector3 finalMove = _moveDirection + _externalForce;

        _characterController.Move(finalMove * Time.deltaTime);

        _externalForce = Vector3.Lerp(_externalForce, Vector3.zero, knockbackDamping * Time.deltaTime);
    }

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
    public void Jump()
    {
        if (_isStunned || !_characterController.isGrounded) return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    // ================= EXTERNAL EFFECTS =================
    public void ApplyKnockback(Vector3 direction, float force)
    {
        _externalForce += direction.normalized * force;
    }

    public void ApplyStun(float duration)
    {
        StopCoroutine(nameof(StunRoutine));
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        _isStunned = true;

        yield return new WaitForSeconds(duration);

        _isStunned = false;
    }
}