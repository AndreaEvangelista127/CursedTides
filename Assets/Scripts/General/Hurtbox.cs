using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private float _multiplier = 1f;

    private IDamageable _health;

    private void Awake()
    {
        _health = GetComponentInParent<IDamageable>(); // PlayerHealth implements IDamageable, so we can get it from the parent
    }

    public void ReceiveDmg(float  dmg)
    {
        float finalDmg = dmg * _multiplier;

        _health.TakeDamage(finalDmg); //This calls the TakeDamage method in PlayerHealth, which reduces the player's health and updates the health bar
        Debug.Log("Final damage calculated");
    }

    
}
