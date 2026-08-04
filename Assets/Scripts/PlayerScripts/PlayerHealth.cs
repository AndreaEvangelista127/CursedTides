using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private CheckPointManager _checkPointManager;
    [SerializeField] private Transform _chestBone; // Used by the ProjectileLauncher to correctly aim to the player´s chest

    public event Action<float> OnHealthChange; //Event to notify listeners of health changes
    public static event Action OnPlayerDeath; //Event to notify listeners of player death
    public Transform ChestBone => _chestBone;

    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth / maxHealth); //Notify HealthBar of the initial health value
    }

    [ContextMenu("Test Take Damage")]
    public void TestTakeDamage()
    {
        TakeDamage(50f);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Ensure health doesn't go below 0 or above max

        OnHealthChange?.Invoke(currentHealth / maxHealth); // Notify the healthbar with the % of health remaining

        if (currentHealth <= 0f)
        {
            Die();
        }
    }


    private void Die()
    {
        OnPlayerDeath?.Invoke(); // Notify listeners of player death
        _checkPointManager.Respawn();
        currentHealth = maxHealth; // Reset health to max after respawning
        OnHealthChange?.Invoke(currentHealth / maxHealth); // Notify HealthBar of the health reset
    }


}
