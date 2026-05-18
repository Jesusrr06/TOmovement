using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Hitboxes")]
    public Hitbox hitboxes;

    private Animator _animator;
    private PlayerMovement _movementOwner;

    [Header("Combat State")]
    private bool _isPunching;
    private bool _isKicking;
    private bool _isShooting;
    private bool _isBlocking;
    private bool _isInEndLag;

    public int playerId;

    // ================= GETTERS =================
    public bool IsBlocking => _isBlocking;

    public bool IsAttacking =>
        _isPunching ||
        _isKicking ||
        _isShooting;

    private void Awake()
    {
        _movementOwner = GetComponent<PlayerMovement>();
        _animator = GetComponent<Animator>();

        FindHitbox();
    }

    private void Update()
    {
        HandlePlayerInput();
        HandleAnimations();
    }

    // ================= INPUT =================
    private void HandlePlayerInput()
    {
        // Prevent all combat while stunned
        if (_movementOwner.isStunned)
        {
            SetBlock(false);
            return;
        }

        // PLAYER 1
        if (playerId == 1)
        {
            if (Input.GetKeyDown(KeyCode.F))
                Punch();

            if (Input.GetKeyDown(KeyCode.G))
                Kick();

            if (Input.GetKeyDown(KeyCode.H))
                Shoot();

            SetBlock(Input.GetKey(KeyCode.E));
        }

        // PLAYER 2
        if (playerId == 2)
        {
            if (Input.GetKeyDown(KeyCode.Keypad1))
                Punch();

            if (Input.GetKeyDown(KeyCode.Keypad2))
                Kick();

            if (Input.GetKeyDown(KeyCode.Keypad3))
                Shoot();

            SetBlock(Input.GetKey(KeyCode.Keypad4));
        }
    }

    // ================= ANIMATIONS =================
    private void HandleAnimations()
    {
        _animator.SetBool("IsBlocking", _isBlocking);
        _animator.SetBool("IsStunned", _movementOwner.isStunned);
    }

    // ================= CAN ATTACK =================
    private bool CanAttack()
    {
        return
            !_isPunching &&
            !_isKicking &&
            !_isShooting &&
            !_isBlocking &&
            !_isInEndLag &&
            !_movementOwner.isStunned;
    }

    // ================= PUNCH =================
    private void Punch()
    {
        Debug.Log("Punch");
        if (!CanAttack()) return;

        _isPunching = true;
        _isInEndLag = true;

        _animator.SetTrigger("IsPunching");

        hitboxes.BeginAttack();

        StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return null;

        hitboxes.EnableArm();

        yield return new WaitForSeconds(0.15f);

        hitboxes.DisableArm();

        _isPunching = false;

        // Endlag
        yield return new WaitForSeconds(0.2f);

        _isInEndLag = false;
    }

    // ================= KICK =================
    private void Kick()
    {
        if (!CanAttack()) return;

        _isKicking = true;
        _isInEndLag = true;

        _animator.SetTrigger("IsKicking");

        hitboxes.BeginAttack();

        StartCoroutine(KickRoutine());
    }

    private IEnumerator KickRoutine()
    {
        yield return null;

        hitboxes.EnableLeg();

        yield return new WaitForSeconds(0.17f);

        hitboxes.DisableLeg();

        _isKicking = false;

        // Endlag
        yield return new WaitForSeconds(0.25f);

        _isInEndLag = false;
    }

    // ================= SHOOT =================
    private void Shoot()
    {
        if (!CanAttack()) return;

        _isShooting = true;
        _isInEndLag = true;

        _animator.SetTrigger("IsShooting");

        StartCoroutine(ShootRoutine());
    }

    private IEnumerator ShootRoutine()
    {
        // Spawn projectile here later

        yield return new WaitForSeconds(0.3f);

        _isShooting = false;

        // Endlag
        yield return new WaitForSeconds(0.3f);

        _isInEndLag = false;
    }

    // ================= BLOCK =================
    private void SetBlock(bool state)
    {
        // Can't block while attacking
        if (IsAttacking)
            state = false;

        _isBlocking = state;
    }

    // ================= HITBOX =================
    private void FindHitbox()
    {
        Hitbox[] hitboxesArray = GetComponentsInChildren<Hitbox>(true);

        foreach (Hitbox hb in hitboxesArray)
        {
            if (hb.armCollider is not null || hb.legCollider is not null)
            {
                hitboxes = hb;

                hitboxes.movementOwner = GetComponent<PlayerMovement>();
                hitboxes.combatOwner = this;

                return;
            }
        }
    }
}