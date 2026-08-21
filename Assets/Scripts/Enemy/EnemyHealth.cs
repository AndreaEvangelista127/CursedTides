using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); 

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// Used for setting the health of the enemy when it is spawned in the editor tool
    /// </summary>
    /// <param name="health"></param>
    public void ApplyHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }

    private void Die()
    {
        AudioManager.Instance?.PlayEnemyDeath();
        Destroy(gameObject);
    }
}
