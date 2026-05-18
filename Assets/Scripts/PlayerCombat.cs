using UnityEngine;
using System.Collections;

/// <summary>
/// Manages combat input, attack windows and hit detection.
/// Enables hit colliders for short windows and applies damage/stun to targets.
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Hitboxes")]
    public Collider armHitbox;
    public Collider legHitbox;

    private Animator _animator;
    private PlayerMovement _movementOwner;

    [Header("Auto-disable (frames)")]
    [Tooltip("Number of frames to keep colliders enabled when activated from this class")]
    public int hitboxActiveFrames = 3;

    [Header("Combat State")]
    private bool _isPunching;
    private bool _isKicking;
    private bool _isShooting;
    private bool _isBlocking;
    private bool _isInEndLag;
    private bool _alreadyHit;

    [Header("Player")]
    public int playerId;

    // ================= GETTERS =================

    public bool IsBlocking => _isBlocking;

    public bool IsAttacking =>
        _isPunching ||
        _isKicking ||
        _isShooting;

    // ================= UNITY =================

    /// <summary>
    /// Cache component references, disable hit colliders and ensure physics settings
    /// on hitbox objects so trigger events or overlap checks work correctly.
    /// </summary>
    private void Awake()
    {
        _movementOwner = GetComponent<PlayerMovement>();
        CharacterController controller = GetComponentInChildren<CharacterController>();
        if (controller != null)
        {
            controller.enabled = true;
        }
        else
        {
            Debug.LogWarning($"No CharacterController found on {gameObject.name} (PlayerCombat.Awake)");
        }
        _animator = GetComponent<Animator>();

        // ensure hit colliders are disabled until an attack is triggered
        if (armHitbox != null)
        {
            armHitbox.enabled = false;
            armHitbox.isTrigger = true;
            Rigidbody rb = armHitbox.GetComponent<Rigidbody>();
            if (rb == null) rb = armHitbox.gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        if (legHitbox != null)
        {
            legHitbox.enabled = false;
            legHitbox.isTrigger = true;
            Rigidbody rb2 = legHitbox.GetComponent<Rigidbody>();
            if (rb2 == null) rb2 = legHitbox.gameObject.AddComponent<Rigidbody>();
            rb2.isKinematic = true;
            rb2.useGravity = false;
        }
    }

    /// <summary>
    /// Per-frame update handling input and animation state.
    /// </summary>
    private void Update()
    {
        HandlePlayerInput();
        HandleAnimations();
    }

    // ================= INPUT =================

    /// <summary>
    /// Processes combat input (punch/kick/shoot/block) for the local player and
    /// prevents actions when stunned. Maps keys per playerId.
    /// </summary>
    private void HandlePlayerInput()
    {
        // no combatir mientras está stun
        if (_movementOwner.isStunned)
        {
            SetBlock(false);
            return;
        }

        // PLAYER 1
        if (playerId == 1)
        {
            if (Input.GetKey(KeyCode.F))
                Punch();

            if (Input.GetKey(KeyCode.G))
                Kick();

            if (Input.GetKey(KeyCode.H))
                Shoot();

            SetBlock(Input.GetKey(KeyCode.E));
        }

        // PLAYER 2
        if (playerId == 2)
        {
            if (Input.GetKey(KeyCode.Keypad1))
                Punch();

            if (Input.GetKey(KeyCode.Keypad2))
                Kick();

            if (Input.GetKey(KeyCode.Keypad3))
                Shoot();

            SetBlock(Input.GetKey(KeyCode.Keypad4));
        }
    }

    // ================= ANIMATIONS =================

    /// <summary>
    /// Updates animator flags for blocking and stunned states originating from combat.
    /// </summary>
    private void HandleAnimations()
    {
        _animator.SetBool("IsBlocking", _isBlocking);
        _animator.SetBool("IsStunned", _movementOwner.isStunned);

       
    }

    // ================= CAN ATTACK =================

    /// <summary>
    /// Returns true if the player is allowed to start a new attack (not stunned, not blocking, etc.).
    /// </summary>
    /// <returns>True when an attack may start.</returns>
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

    /// <summary>
    /// Triggers the punch animation and starts the punch attack window coroutine.
    /// </summary>
    private void Punch()
    {
        if (!CanAttack())
            return;

        _isPunching = true;
        _isInEndLag = true;
        _animator.SetTrigger("IsPunching");

        StartCoroutine(PunchRoutine());
    }

    /// <summary>
    /// Attack coroutine for punching: enables the punch collider briefly and handles endlag.
    /// </summary>
    /// <returns>IEnumerator for StartCoroutine.</returns>
    private IEnumerator PunchRoutine()
    {
        yield return null;

        _alreadyHit = false;

        if (armHitbox != null)
        {
            StartCoroutine(EnableColliderForFrames(armHitbox, hitboxActiveFrames));
            TryDetectHit(armHitbox);
        }

        yield return new WaitForSeconds(0.15f);

        _isPunching = false;

        yield return new WaitForSeconds(0.4f);

        _isInEndLag = false;
    }

    // ================= KICK =================

    /// <summary>
    /// Triggers the kick animation and starts the kick attack window coroutine.
    /// </summary>
    private void Kick()
    {
        if (!CanAttack())
            return;

        _isKicking = true;
        _isInEndLag = true;
        _animator.SetTrigger("IsKicking");

        StartCoroutine(KickRoutine());
    }

    /// <summary>
    /// Attack coroutine for kicking: enables the kick collider briefly and handles endlag.
    /// </summary>
    /// <returns>IEnumerator for StartCoroutine.</returns>
    private IEnumerator KickRoutine()
    {
        yield return null;

        _alreadyHit = false;

        if (legHitbox != null)
        {
            StartCoroutine(EnableColliderForFrames(legHitbox, hitboxActiveFrames));
            TryDetectHit(legHitbox);
        }

        yield return new WaitForSeconds(0.17f);

        _isKicking = false;

        yield return new WaitForSeconds(0.3f);

        _isInEndLag = false;
    }

    // ================= SHOOT =================

    /// <summary>
    /// Starts the shooting action and handles its endlag. (No hit collider in current implementation.)
    /// </summary>
    private void Shoot()
    {
        if (!CanAttack())
            return;

        _isShooting = true;
        _isInEndLag = true;
        _animator.SetTrigger("IsShooting");

        StartCoroutine(ShootRoutine());
    }

    /// <summary>
    /// Simple coroutine for shoot timing and endlag handling.
    /// </summary>
    /// <returns>IEnumerator for StartCoroutine.</returns>
    private IEnumerator ShootRoutine()
    {
        yield return new WaitForSeconds(0.3f);

        _isShooting = false;

        yield return new WaitForSeconds(0.4f);

        _isInEndLag = false;
    }

    /// <summary>
    /// Trigger callback invoked when an enabled hit collider intersects another trigger/collider.
    /// Applies damage and stun to a valid PlayerMovement target depending on whether they are blocking.
    /// </summary>
    /// <param name="other">The other collider that entered the trigger.</param>
    private void OnTriggerEnter(Collider other)
    {
        if (_alreadyHit) return;

        PlayerMovement target = other.GetComponentInParent<PlayerMovement>();
        if (target == null || target == _movementOwner) return;

        _alreadyHit = true;

        float finalDamage = 10f;
        if (target.GetComponentInChildren<PlayerCombat>()?.IsBlocking == true)
        {
            finalDamage *= 0.3f; // reduced while holding block        }
        else        {            target.ApplyStun(0.5f, PlayerMovement.StunType.Hitstun);        }
        Health hp = target.GetComponentInChildren<Health>();
        if (hp != null)        {            hp.TakeDamage(finalDamage);        }
    }
    // ================= BLOCK =================

    /// <summary>
    /// Enables or disables blocking. Blocking is disabled while performing attacks.
    /// Blocking state is used by incoming attacks to reduce damage.
    /// </summary>
    /// <param name="state">True to enable blocking, false to disable.</param>
    private void SetBlock(bool state)
    {
        // no bloquear mientras atacas
        if (IsAttacking)
            state = false;

        _isBlocking = state;
    }

    /// <summary>
    /// Temporarily enables a collider for a given number of frames to act as an attack window.
    /// Resets the per-attack hit flag so the collider can register a single hit during the window.
    /// </summary>
    /// <param name="col">Collider to enable.</param>
    /// <param name="frames">Number of frames to keep the collider active.</param>
    /// <returns>IEnumerator for StartCoroutine.</returns>
    private IEnumerator EnableColliderForFrames(Collider col, int frames)
    {
        if (col == null) yield break;
        // reset per-attack hit flag so this collider can register one hit during the window
        _alreadyHit = false;
        col.enabled = true;
        for (int i = 0; i < Mathf.Max(1, frames); i++)
            yield return new WaitForEndOfFrame();        col.enabled = false;    }

    /// <summary>
    /// Performs an immediate overlap check using the collider's bounds to detect hits
    /// in case trigger callbacks are not fired in the same frame. Applies damage/stun
    /// consistently with OnTriggerEnter.
    /// </summary>
    /// <param name="col">The attack collider used for overlap checks.</param>
    private void TryDetectHit(Collider col)
    {        if (col == null) return;
        Collider[] hits = Physics.OverlapBox(col.bounds.center, col.bounds.extents, col.transform.rotation);        foreach (var c in hits)        {            if (c.transform.root == transform.root) continue;            PlayerMovement target = c.GetComponentInParent<PlayerMovement>();            if (target == null || target == _movementOwner) continue;            if (_alreadyHit) continue;            _alreadyHit = true;            Health hp = target.GetComponentInChildren<Health>();            if (hp != null)            {                float finalDamage = 10f;
                PlayerCombat targetCombat = target.GetComponentInChildren<PlayerCombat>();
                if (targetCombat != null && targetCombat.IsBlocking)
                {
                    finalDamage *= 0.3f;
                }
                else
                {
                    target.ApplyStun(0.5f, PlayerMovement.StunType.Hitstun);
                }
                hp.TakeDamage(finalDamage);
                Debug.Log($"{gameObject.name} hit {target.name} for {finalDamage} (overlap)");                Debug.Log($"{gameObject.name} hit {target.name} for 10 (overlap)");            }            break;        }    }    private void OnDisable()    {        // ensure colliders are off when object is disabled/destroyed        if (armHitbox != null) armHitbox.enabled = false;        if (legHitbox != null) legHitbox.enabled = false;    }
}