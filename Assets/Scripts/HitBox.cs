using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Owner")]
    public PlayerMovement movementOwner;
    public PlayerCombat combatOwner;

    [Header("Colliders")]
    public Collider armCollider;
    public Collider legCollider;

    private bool _hasHitThisAttack = false;
    private bool _isAttacking = false;

    [Header("Combat")]
    public float damage = 10f;

    private void Awake()
    {
        if (movementOwner is null)
            movementOwner = GetComponentInParent<PlayerMovement>();

        if (combatOwner is null)
            combatOwner = GetComponentInParent<PlayerCombat>();

        DisableAll();
    }

    // ================= OWNER =================
    public void SetOwner(PlayerMovement p)
    {
        movementOwner = p;
    }

    // ================= ATTACK =================
    public void BeginAttack()
    {
        _hasHitThisAttack = false;
        _isAttacking = true;
    }

    public void EndAttack()
    {
        _isAttacking = false;
    }

    // ================= HIT DETECTION =================
    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttacking || _hasHitThisAttack)
            return;

        // evitar golpearse a sí mismo
        if (other.transform.IsChildOf(movementOwner.transform))
            return;

        PlayerMovement targetMovement =
            other.GetComponentInParent<PlayerMovement>();

        if (targetMovement is null)
            return;

        PlayerCombat targetCombat =
            other.GetComponentInParent<PlayerCombat>();

        if (targetCombat is null)
            return;

        _hasHitThisAttack = true;

        float finalDamage = damage;

        // ================= BLOCK =================
        if (targetCombat.IsBlocking)
        {
            finalDamage *= 0.3f;
            Debug.Log($"{targetCombat.name} bloqueó el ataque!");
        }

        // ================= DAMAGE =================
        Health hp = targetMovement.GetComponentInChildren<Health>();

        if (hp is not null)
        {
            hp.TakeDamage(finalDamage);
        }

        Debug.Log($"{movementOwner.name} hit {targetMovement.name} for {finalDamage} damage");
    }

    // ================= COLLIDERS =================
    public void EnableArm()
    {
        if (armCollider is not null)
            armCollider.enabled = true;
    }

    public void DisableArm()
    {
        if (armCollider is not null)
            armCollider.enabled = false;
    }

    public void EnableLeg()
    {
        if (legCollider is not null)
            legCollider.enabled = true;
    }

    public void DisableLeg()
    {
        if (legCollider is not null)
            legCollider.enabled = false;
    }

    public void EnableAll()
    {
        EnableArm();
        EnableLeg();
    }

    public void DisableAll()
    {
        DisableArm();
        DisableLeg();
    }
}