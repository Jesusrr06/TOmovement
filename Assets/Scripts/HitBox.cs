using UnityEngine;
using UnityEngine.InputSystem;

public class Hitbox : MonoBehaviour
{
    [Header("Owner")]
    public PC owner;

    [Header("Colliders")]
    public Collider armCollider;
    public Collider legCollider;

    private bool _hasHitThisAttack = false;
    private bool _isAttacking = false;

    [Header("Combat")]
    public float damage = 10f;

    private void Awake()
    {
        if (owner is null)
            owner = GetComponentInParent<PC>();

        DisableAll();
    }

    public void SetOwner(PC p)
    {
        owner = p;
    }

    public void BeginAttack()
    {
        _hasHitThisAttack = false;
        _isAttacking = true;
        // Debug para indicar qué tecla activó el ataque
    }

    public void EndAttack()
    {
        _isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAttacking || _hasHitThisAttack) return;

        if (other.transform.IsChildOf(owner.transform)) return;

        PC target = other.GetComponentInParent<PC>();
        if (target is null) return;

        Health hp = target.GetComponentInChildren<Health>();
        if (hp is not null)
        {
            _hasHitThisAttack = true;
            hp.TakeDamage(damage);
            Debug.Log($"{owner.name} hit {target.name} for {damage} damage");
        }
    }

    // = Habilitar/Deshabilitar colliders =
    public void EnableArm() { if (armCollider is not null) armCollider.enabled = true; }
    public void DisableArm() { if (armCollider is not null) armCollider.enabled = false; }
    public void EnableLeg() { if (legCollider is not null) legCollider.enabled = true; }
    public void DisableLeg() { if (legCollider is not null) legCollider.enabled = false; }
    public void EnableAll() { EnableArm(); EnableLeg(); }
    public void DisableAll() { DisableArm(); DisableLeg(); }
}