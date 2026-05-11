using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class PC : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Jump")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Combat")]
    [SerializeField] private bool canMoveWhileAttacking = false;

    public GameObject punchHitbox;
    public Health enemyHealth;

    [Header("Player Info")]
    public int playerId;

    private Vector3 _moveDirection;
    private Vector2 _moveInput;
    private float _verticalVelocity;

    private bool _isPunching;
    private bool _isKicking;
    private bool _isShooting;
    private bool _isBlocking;

    private Camera _cam;

    private void Start()
    {
        _cam = Camera.main;
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();

        if (punchHitbox != null)
            punchHitbox.GetComponent<Hitbox>().owner = this;

        FindEnemyHealth();
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        HandleRotation();
        HandleGravity();
        HandleAnimations();
    }
    private void Awake()
    {
        if (punchHitbox == null)
        {
            Transform t = transform.Find("PunchHitbox");

            if (t != null)
                punchHitbox = t.gameObject;
            else
                Debug.LogError("PunchHitbox NO encontrado en prefab");
        }
    }
    // ========================= INPUT =========================
    private void HandleInput()
    {
        _moveInput = Vector2.zero;

        if (playerId == 1)
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal");
            _moveInput.y = Input.GetAxisRaw("Vertical");

            if (Input.GetKeyDown(KeyCode.Space)) Jump();
            if (Input.GetKeyDown(KeyCode.F)) Punch();
            if (Input.GetKeyDown(KeyCode.G)) Kick();
            if (Input.GetKeyDown(KeyCode.H)) Shoot();

            Block(Input.GetKey(KeyCode.E));
        }
        else
        {
            _moveInput.x = Input.GetAxisRaw("Horizontal2");
            _moveInput.y = Input.GetAxisRaw("Vertical2");

            if (Input.GetKeyDown(KeyCode.Keypad0)) Jump();
            if (Input.GetKeyDown(KeyCode.Keypad1)) Punch();
            if (Input.GetKeyDown(KeyCode.Keypad2)) Kick();
            if (Input.GetKeyDown(KeyCode.Keypad3)) Shoot();

            Block(Input.GetKey(KeyCode.Keypad4));
        }
    }

    // ========================= MOVEMENT =========================
    private void HandleMovement()
    {
        if ((_isPunching && !canMoveWhileAttacking) ||
            (_isKicking && !canMoveWhileAttacking) ||
            (_isShooting && !canMoveWhileAttacking) ||
            _isBlocking)
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
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    // ========================= ROTATION =========================
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

    // ========================= GRAVITY =========================
    private void HandleGravity()
    {
        if (_characterController.isGrounded && _verticalVelocity < 0)
            _verticalVelocity = -2f;

        _verticalVelocity += gravity * Time.deltaTime;
    }

    // ========================= ANIMATIONS =========================
    private void HandleAnimations()
    {
        Vector3 horizontal = _moveDirection;
        horizontal.y = 0;

        _animator.SetBool("IsRunning", horizontal.sqrMagnitude > 0.1f);
        _animator.SetBool("IsJumping", !_characterController.isGrounded && _verticalVelocity > 0);
        _animator.SetBool("IsFalling", !_characterController.isGrounded && _verticalVelocity < 0);

        _animator.SetBool("IsKicking", _isKicking);
        _animator.SetBool("IsPunching", _isPunching);
        _animator.SetBool("IsBlocking", _isBlocking);
    }

    // ========================= ACTIONS =========================
    private void Jump()
    {
        if (!_characterController.isGrounded) return;

        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    private void Block(bool state)
    {
        _isBlocking = state;
        _animator.SetBool("IsBlocking", state);
    }

    private void Punch()
    {
        if (_isPunching) return;

        _isPunching = true;
        _animator.SetTrigger("IsPunching");

        StartCoroutine(EnableHitbox());

        StartCoroutine(ResetPunch());
    }

    private void Kick()
    {
        if (_isKicking) return;

        _isKicking = true;
        _animator.SetTrigger("IsKicking");

        StartCoroutine(ResetKick());
    }

    private void Shoot()
    {
        if (_isShooting) return;

        _isShooting = true;
        _animator.SetTrigger("IsShooting");

        StartCoroutine(ResetShoot());
    }

    // ========================= HITBOX =========================
    private IEnumerator EnableHitbox()
    {
        if (punchHitbox == null) yield break;

        punchHitbox.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        punchHitbox.SetActive(false);
    }

    // ========================= RESET =========================
    private IEnumerator ResetPunch()
    {
        yield return new WaitForSeconds(0.25f);
        _isPunching = false;
    }

    private IEnumerator ResetKick()
    {
        yield return new WaitForSeconds(0.25f);
        _isKicking = false;
    }

    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.25f);
        _isShooting = false;
    }

    // ========================= ENEMY FIND =========================
    private void FindEnemyHealth()
    {
        PC[] players = FindObjectsByType<PC>();

        foreach (PC p in players)
        {
            if (p != this)
                enemyHealth = p.GetComponent<Health>();
        }
    }
}