using UnityEngine;
using System.Collections;

public class PlayerCombat : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private bool canMoveWhileAttacking = false;

    [Header("Hitboxes")]
    public Hitbox hitboxes;

    private Animator _animator;

    private bool _isPunching;
    private bool _isKicking;
    private bool _isShooting;
    private bool _isBlocking;
    private bool _isStunned;

    // ================= GETTERS =================
    public bool IsBlocking => _isBlocking;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        FindHitbox();
    }

    // ================= PUNCH =================
    public void Punch()
    {
        if (_isStunned || _isBlocking) return;

        _isPunching = true;
        _animator.SetTrigger("IsPunching");

        hitboxes.BeginAttack();
        StartCoroutine(PunchRoutine());
    }

    private IEnumerator PunchRoutine()
    {
        yield return null;

        hitboxes.EnableArm();

        yield return new WaitForSeconds(0.25f);

        hitboxes.DisableArm();
        _isPunching = false;
    }

    // ================= KICK =================
    public void Kick()
    {
        if (_isStunned || _isBlocking) return;

        _isKicking = true;
        _animator.SetTrigger("IsKicking");

        hitboxes.BeginAttack();
        StartCoroutine(KickRoutine());
    }

    private IEnumerator KickRoutine()
    {
        yield return null;

        hitboxes.EnableLeg();

        yield return new WaitForSeconds(0.25f);

        hitboxes.DisableLeg();
        _isKicking = false;
    }

    // ================= SHOOT =================
    public void Shoot()
    {
        if (_isStunned || _isBlocking) return;

        _isShooting = true;
        _animator.SetTrigger("IsShooting");

        StartCoroutine(ResetShoot());
    }

    private IEnumerator ResetShoot()
    {
        yield return new WaitForSeconds(0.25f);
        _isShooting = false;
    }

    // ================= BLOCK =================
    public void SetBlock(bool state)
    {
        if (_isPunching || _isKicking || _isShooting)
            state = false;

        _isBlocking = state;
    }

    // ================= STUN =================
    public void SetStunned(bool state)
    {
        _isStunned = state;

        if (state)
        {
            _isBlocking = false;
            _isPunching = false;
            _isKicking = false;
            _isShooting = false;
        }
    }

    // ================= HITBOX =================
    private void FindHitbox()
    {
        Hitbox[] hitboxesArray = GetComponentsInChildren<Hitbox>(true);

        foreach (Hitbox hb in hitboxesArray)
        {
            if (hb.armCollider != null || hb.legCollider != null)
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