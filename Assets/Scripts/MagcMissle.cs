using UnityEngine;

public class MagicMissile : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 15f;
    public float lifeTime = 3f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health hp = other.GetComponentInParent<Health>();
        
        
        if (hp != null)
        {
            hp.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}