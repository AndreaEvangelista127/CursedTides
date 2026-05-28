using Ilumisoft.HealthSystem;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    [SerializeField] private float _multiplier = 1f;

    private IDamageable _health;

    public void ReceiveDmg(float  dmg)
    {
        float finalDmg = dmg * _multiplier;

        _health.TakeDamage(finalDmg);
        Debug.Log("Final damage calculated");
    }

    
}
