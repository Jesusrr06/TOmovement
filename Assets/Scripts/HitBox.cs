using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [Header("Owner")]
    public PC owner;

    [Header("Colliders")]
    public Collider armCollider;
    public Collider legCollider;

    private bool _hasHitThisAttack = false;

    [Header("Combat")]
    public float damage = 10f;

    private void Awake()
    {
        // Si no se asignó owner desde el manager
        if (owner == null)
            owner = GetComponentInParent<PC>();

        // Desactivamos colliders al inicio para evitar daño al spawnear
        if (armCollider != null) armCollider.enabled = false;
        if (legCollider != null) legCollider.enabled = false;
    }

    public void SetOwner(PC p)
    {
        owner = p;
    }

    // Llamar cada vez que empieza un ataque
    public void BeginAttack()
    {
        _hasHitThisAttack = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_hasHitThisAttack) return;

        PC target = other.GetComponentInParent<PC>();
        if (target == null) return;

        // Ignorar al dueño
        if (target == owner) return;


        Health hp = target.GetComponentInChildren<Health>();
        if (hp != null)
        {
            _hasHitThisAttack = true;
            Debug.Log(owner.name + " golpea a " + target.name);
            hp.TakeDamage(damage);
        }
    }

    // ================= Habilitar/Deshabilitar colliders =================
    public void EnableArm() { if (armCollider is not null) armCollider.enabled = true; }
    public void DisableArm() { if (armCollider is not null) armCollider.enabled = false; }
    public void EnableLeg() { if (legCollider is not null) legCollider.enabled = true; }
    public void DisableLeg() { if (legCollider is not null) legCollider.enabled = false; }
    public void EnableAll() { EnableArm(); EnableLeg(); }
    public void DisableAll() { DisableArm(); DisableLeg(); }
}