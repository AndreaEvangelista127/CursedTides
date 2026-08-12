using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamageable
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private CheckPointManager _checkPointManager;
    [SerializeField] private Transform _chestBone; // Used by the ProjectileLauncher to correctly aim to the player�s chest

    // Events
    public event Action<float> OnHealthChange; //Event to notify listeners of health changes
    public static event Action OnPlayerDeath; //Event to notify listeners of player death
    // Events for healing
    public static event Action OnPlayerHealing; 
    public static event Action OnPlayerStopHealing;
    // Event for burning
    public static event Action OnPlayerBurning;
    public static event Action OnPlayerStopBurning;

    // Properties
    public Transform ChestBone => _chestBone;
    public bool IsBurning { get; private set; } = false;


    private void Awake()
    {
        currentHealth = maxHealth;
        OnHealthChange?.Invoke(currentHealth / maxHealth); //Notify HealthBar of the initial health value
    }

    public static void TriggerHealing() => OnPlayerHealing?.Invoke();
    public static void TriggerStopHealing() => OnPlayerStopHealing?.Invoke();
    public static void TriggerBurning() => OnPlayerBurning?.Invoke();
    public static void TriggerStopBurning() => OnPlayerStopBurning?.Invoke();

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

    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth); // Ensure health doesn't go below 0 or above max
        OnHealthChange?.Invoke(currentHealth / maxHealth); // Notify the healthbar with the % of health remaining
    }

    public void SetBurningState(bool isBurning)
    {
        IsBurning = isBurning;
    }

    private void Die()
    {
        OnPlayerDeath?.Invoke(); // Notify listeners of player death
        GameManager.Instance?.OnPlayerDeath();
    }

}
