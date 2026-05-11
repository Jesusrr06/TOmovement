using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public PC owner;
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        PC target = other.GetComponent<PC>();

        if (target == owner) return;

        Health hp = other.GetComponent<Health>();

        if (hp != null)
        {
            hp.TakeDamage(damage);
        }
    }
}