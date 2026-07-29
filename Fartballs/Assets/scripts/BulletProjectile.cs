using UnityEngine;

// Attach this to a bullet PREFAB (not a scene object).
// The bullet flies forward and damages whatever it hits.
// Speed and damage get overwritten automatically by PlayerShooter/EnemyAI when it's spawned,
// but the defaults below are used if you test the prefab on its own.

[RequireComponent(typeof(Rigidbody))]
public class BulletProjectile : MonoBehaviour
{
    public float speed = 60f;
    public int damage = 20;
    public float lifeTime = 5f; // seconds before the bullet auto-destroys, so they don't pile up forever

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed; // use rb.velocity if on an older Unity version

        Destroy(gameObject, lifeTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        var damageable = collision.collider.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage);
        }

        // Optional: spawn an impact effect here before destroying

        Destroy(gameObject);
    }
}
