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
        Debug.Log($"Ouch i took {damage} damage");
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); 

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    // Used for setting the health of the enemy when it is spawned in the editor tool
    public void ApplyHealth(float health)
    {
        maxHealth = health;
        currentHealth = health;
    }

    private void Die()
    {
        Debug.Log("Im dead");
        Destroy(gameObject);
    }
}
