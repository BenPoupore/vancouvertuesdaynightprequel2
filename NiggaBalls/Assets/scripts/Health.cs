using UnityEngine;
using UnityEngine.Events;

// Put this on ANY object that should be able to take damage and die:
// the Player, the Enemy, breakable crates, etc.
// It implements IDamageable, which your PlayerShooter and EnemyAI can damage.

public class Health : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("Death Settings")]
    public bool destroyOnDeath = true;
    public bool destroyRootObject = true;
    public float destroyDelay = 0f;

    [Header("Events")]
    public UnityEvent onDamaged;
    public UnityEvent onDeath;

    private bool isDead = false;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        onDamaged?.Invoke();

        Debug.Log(gameObject.name + " took " + amount + " damage. HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log(gameObject.name + " died.");

        onDeath?.Invoke();

        // Player death should not destroy the player by default.
        if (gameObject.CompareTag("Player"))
        {
            Debug.Log("GAME OVER");
            return;
        }

        // Enemy / object death sound.
        EnemyDeathSound deathSound = GetComponent<EnemyDeathSound>();

        if (deathSound == null)
            deathSound = GetComponentInChildren<EnemyDeathSound>();

        if (deathSound == null && transform.root != null)
            deathSound = transform.root.GetComponentInChildren<EnemyDeathSound>();

        if (deathSound != null)
            deathSound.PlayDeathSound();

        if (destroyOnDeath)
        {
            GameObject objectToDestroy = destroyRootObject ? transform.root.gameObject : gameObject;
            Destroy(objectToDestroy, destroyDelay);
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }
}
