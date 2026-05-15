using System;
using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{

    [Header("Hitboxes")]
    public Hitbox hitboxes;

    private Animator _animator;

    private bool _isPunching;
    private bool _isKicking;
    private bool _isShooting;
    private bool _isBlocking;
    private PlayerMovement _movementOwner;
    private bool _isInEndLag;
    public  int playerId;

    // ================= GETTERS =================
    public bool IsBlocking => _isBlocking;

    private void Awake()
    {         _movementOwner = GetComponent<PlayerMovement>();

        _animator = GetComponent<Animator>();
        FindHitbox();
    }

    private void Update()
    {
        HandlePlayerInput();
        HandleAnimations();
        
    }
    private void HandleAnimations()
    {
        _animator.SetBool("IsBlocking", _isBlocking);

        _animator.SetBool("IsStunned", _movementOwner.isStunned);
    }

    private void HandlePlayerInput()
    {
        if (_movementOwner.isStunned)
        {
            SetBlock(false);
            return;
        }

        // PLAYER 1
        if (playerId == 1)
        {

            if (Input.GetKey(KeyCode.F)) Punch();
            if (Input.GetKey(KeyCode.G)) Kick();
            if (Input.GetKey(KeyCode.H)) Shoot();
            SetBlock(Input.GetKey(KeyCode.E));
        }

        // PLAYER 2
        if (playerId == 2)
        {
            if (Input.GetKey(KeyCode.Keypad1)) Punch();
            if (Input.GetKey(KeyCode.Keypad2)) Kick();
            if (Input.GetKey(KeyCode.Keypad3)) Shoot();
            SetBlock(Input.GetKey(KeyCode.Keypad4));
        }

    }
    

    // ================= PUNCH =================
    private void Punch()
    {
        if ( _isBlocking) return;

        _isPunching = true;
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
    }

    // ================= KICK =================
    private void Kick()
    {
        if (_isBlocking) return;

        _isKicking = true;
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
    }

    // ================= SHOOT =================
    private void Shoot()
    {
        if (_isBlocking) return;

        _isShooting = true;
        _animator.SetTrigger("IsShooting");

        StartCoroutine(ResetShoot());
    }

    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.15f);
        _isShooting = false;
    }

    // ================= BLOCK =================
    private void SetBlock(bool state)
    {
        if (_isPunching || _isKicking || _isShooting)
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

                // ✔ actualización correcta para arquitectura nueva
                hitboxes.movementOwner = GetComponent<PlayerMovement>();
                hitboxes.combatOwner = this;

                return;
            }
        }
    }
}